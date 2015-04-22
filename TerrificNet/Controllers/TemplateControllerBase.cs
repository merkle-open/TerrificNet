using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dependencies;
using TerrificNet.Dispatcher;
using TerrificNet.Models.ErrorHandling;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.IO;
using TerrificNet.ViewEngine.TemplateHandler.UI;
using Veil;
using Veil.Parser;

namespace TerrificNet.Controllers
{
    public class TemplateControllerBase : ApiController, IDependencyResolverAware
    {
        protected TemplateControllerBase()
        {
        }

        protected async Task<HttpResponseMessage> View(PageViewDefinition viewDefinition)
        {
            var dependencyResolver = ((IDependencyResolverAware)this).DependencyResolver;
            var viewEngine = (IViewEngine)dependencyResolver.GetService(typeof(IViewEngine));

            if (viewDefinition.TemplateInfo == null)
            {
                var repo = (ITemplateRepository)dependencyResolver.GetService(typeof(ITemplateRepository));
                viewDefinition.TemplateInfo = await repo.GetTemplateAsync(viewDefinition.Template).ConfigureAwait(false);
            }

            return View(viewEngine, viewDefinition);
        }

        protected HttpResponseMessage View(IViewEngine viewEngine, IPageViewDefinition siteDefinition)
        {
            var message = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content =
                    new PushStreamContent((o, c, t) => WriteToStream(o, viewEngine, siteDefinition),
                        new MediaTypeHeaderValue("text/html"))
            };
            return message;
        }

        private async Task WriteToStream(Stream outputStream, IViewEngine viewEngine, IPageViewDefinition viewDefinition)
        {
            using (var writer = new StreamWriter(outputStream))
            {
                SourceLocation errorLocation = null;
                Exception error = null;
                try
                {
					//await viewDefinition.RenderAsync(viewEngine, writer).ConfigureAwait(false);
                    viewDefinition.Render(viewEngine, writer);
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
                catch (Exception ex)
                {
                    error = ex;
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
                .CreateViewAsync(templateInfo, typeof(ErrorViewModel));

            if (location == null)
            {
                var modelWithoutLocation = new ErrorViewModel
                {
                    ErrorMessage = error.Message,
                    Details = error.StackTrace
                };
                await view.RenderAsync(modelWithoutLocation, new RenderingContext(writer));
                return;
            }

            var templateRepository = (ITemplateRepository)this.Resolver.GetService(typeof(ITemplateRepository));
            var sourceTemplate = await templateRepository.GetTemplateAsync(location.TemplateId);
            string sourceTemplateSource;

            using (var reader = new StreamReader(sourceTemplate.Open()))
            {
                sourceTemplateSource = await reader.ReadToEndAsync();
            }

            var model = new ErrorViewModel
            {
                TemplateId = location.TemplateId,
                ErrorMessage = error.Message,
                Details = error.StackTrace,
                Before = sourceTemplateSource.Substring(0, location.Index),
                Node = sourceTemplateSource.Substring(location.Index, location.Length),
                After = sourceTemplateSource.Substring(location.Index + location.Length),
                Text = HttpUtility.JavaScriptStringEncode(sourceTemplateSource),
                Range = GetRange(sourceTemplateSource, location)
            };

            await view.RenderAsync(model, new RenderingContext(writer));
        }

        private static ErrorRange GetRange(string sourceTemplateSource, SourceLocation location)
        {
            int lineNumber = 0;
            int idx = 0;
            int lastIdx = 0;
            while ((idx = sourceTemplateSource.IndexOf('\n', idx)) >= 0 && idx < location.Index)
            {
                lineNumber++;
                idx++;
                lastIdx = idx;
            }

            int startLineNumber = lineNumber;
            int startIdx = lastIdx;
            while ((idx = sourceTemplateSource.IndexOf('\n', idx)) >= 0 && idx < location.Index + location.Length)
            {
                lineNumber++;
                idx++;
                lastIdx = idx;                
            }

            return new ErrorRange
            {
                StartRow = startLineNumber,
                StartColumn = location.Index - startIdx,
                EndRow = lineNumber,
                EndColumn = location.Index - lastIdx + location.Length
            };
        }

        protected IDependencyResolver Resolver { get { return ((IDependencyResolverAware) this).DependencyResolver; }}

        IDependencyResolver IDependencyResolverAware.DependencyResolver { get; set; }
    }
}