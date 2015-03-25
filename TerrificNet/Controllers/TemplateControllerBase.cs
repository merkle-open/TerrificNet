using System;
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
using TerrificNet.ViewEngine.IO;
using TerrificNet.ViewEngine.ViewEngines;
using Veil;
using Veil.Parser;

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

        private async Task WriteToStream(IView view, object model, Stream outputStream, HttpContent content, TransportContext context)
        {
            using (var writer = new StreamWriter(outputStream))
            {
                SourceLocation errorLocation = null;
                Exception error = null;
                try
                {
                    await view.RenderAsync(model, new RenderingContext(writer));
                }
                catch (VeilParserException ex)
                {
                    error = ex;
                    errorLocation = ex.Location;
                }
                catch (VeilCompilerException ex)
                {
                    error = ex;
                    errorLocation = ex.Node.Location;
                }
                
                if (error != null)
                    await GetErrorPage(writer, error, errorLocation);
            }
        }

        protected async Task GetErrorPage(StreamWriter writer, Exception error, SourceLocation location)
        {
            var fileSystem = new EmbeddedResourceFileSystem(typeof(WebInitializer).Assembly);
            string content;
            using (var reader = new StreamReader(fileSystem.OpenRead(PathInfo.Create("Core/error_partial.html"))))
            {
                content = await reader.ReadToEndAsync();
            }

            var templateInfo = new StringTemplateInfo("error", content);

            var view = await ((IViewEngine)this.Resolver.GetService(typeof(IViewEngine)))
                .CreateViewAsync(templateInfo, typeof(TemplateController.ErrorViewModel));

            var templateRepository = (ITemplateRepository)this.Resolver.GetService(typeof(ITemplateRepository));
            var sourceTemplate = await templateRepository.GetTemplateAsync(location.TemplateId);
            string sourceTemplateSource;

            using (var reader = new StreamReader(sourceTemplate.Open()))
            {
                sourceTemplateSource = await reader.ReadToEndAsync();
            }

            var model = new TemplateController.ErrorViewModel
            {
                TemplateId = location.TemplateId,
                ErrorMessage = error.Message,
                Details = error.StackTrace,
                Before = sourceTemplateSource.Substring(0, location.Index),
                Node = sourceTemplateSource.Substring(location.Index, location.Length),
                After = sourceTemplateSource.Substring(location.Index + location.Length)
            };

            await view.RenderAsync(model, new RenderingContext(writer));
        }

        protected IDependencyResolver Resolver { get { return ((IDependencyResolverAware) this).DependencyResolver; }}

        IDependencyResolver IDependencyResolverAware.DependencyResolver { get; set; }
    }
}