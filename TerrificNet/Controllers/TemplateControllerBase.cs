using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Dependencies;
using TerrificNet.Dispatcher;
using TerrificNet.ViewEngine;

namespace TerrificNet.Controllers
{
    public class TemplateControllerBase : ApiController, IDependencyResolverAware
    {
        protected TemplateControllerBase()
        {
        }

        protected HttpResponseMessage View(string viewName, object model)
        {
            var dependencyResolver = ((IDependencyResolverAware)this).DependencyResolver;
            var templateRepository = (ITemplateRepository)dependencyResolver.GetService(typeof(ITemplateRepository));
            var viewEngine = (IViewEngine)dependencyResolver.GetService(typeof(IViewEngine));

            IView view;
            TemplateInfo templateInfo;
            if (!templateRepository.TryGetTemplate(viewName, string.Empty, out templateInfo) ||
                !viewEngine.TryCreateView(templateInfo, model.GetType(), out view))
                return new HttpResponseMessage(HttpStatusCode.NotFound);

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