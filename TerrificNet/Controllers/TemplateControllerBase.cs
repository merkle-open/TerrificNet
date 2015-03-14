using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dependencies;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TerrificNet.Dispatcher;
using TerrificNet.ViewEngine;
using Veil;

namespace TerrificNet.Controllers
{
    public class TemplateControllerBase : ApiController, IDependencyResolverAware
    {
        protected TemplateControllerBase()
        {
        }

        protected async Task<HttpResponseMessage> View(string viewName, object model)
        {
            var serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            //model = JObject.FromObject(model, serializer);

            var dependencyResolver = ((IDependencyResolverAware)this).DependencyResolver;
            var templateRepository = (ITemplateRepository)dependencyResolver.GetService(typeof(ITemplateRepository));
            var viewEngine = (IViewEngine)dependencyResolver.GetService(typeof(IViewEngine));

            var templateInfo = await templateRepository.GetTemplateAsync(viewName).ConfigureAwait(false);
            if (templateInfo == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            var view = await viewEngine.CreateViewAsync(templateInfo).ConfigureAwait(false);
            return View(view, model);
        }

        protected HttpResponseMessage View(IView view, object model)
        {
            using (var writer = new StringWriter())
            {
                view.Render(model, new RenderingContext(writer));

                var message = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(writer.ToString()) };
                message.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                return message;
            }
        }

        IDependencyResolver IDependencyResolverAware.DependencyResolver { get; set; }
    }
}