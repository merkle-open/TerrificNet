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

            var view = await viewEngine.CreateViewAsync(templateInfo, model.GetType()).ConfigureAwait(false);
            return View(view, model);
        }

        protected HttpResponseMessage View(IView view, object model)
        {
            var message = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content =
                    new PushStreamContent((o, c, t) => WriteToStream(view, model, o, c, t),
                        new MediaTypeHeaderValue("text/html"))
            };
            return message;
        }

        private static async Task WriteToStream(IView view, object model, Stream outputStream, HttpContent content, TransportContext context)
        {
            using (var writer = new StreamWriter(outputStream))
            {
                await view.RenderAsync(model, new RenderingContext(writer));
            }
        }

        IDependencyResolver IDependencyResolverAware.DependencyResolver { get; set; }
    }
}