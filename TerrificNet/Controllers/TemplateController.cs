using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.TemplateHandler.UI;
using Veil.Parser;

namespace TerrificNet.Controllers
{
    public class TemplateController : TemplateControllerBase
    {
        private readonly IViewEngine _viewEngine;
        private readonly TerrificViewDefinitionRepository _viewRepository;

        public TemplateController(IViewEngine viewEngine, TerrificViewDefinitionRepository viewRepository)
        {
            _viewEngine = viewEngine;
            _viewRepository = viewRepository;
        }

        [HttpGet]
        public HttpResponseMessage Get(string path)
        {
            SourceLocation errorLocation;
            Exception error;
            //try
            //{
            return GetInternal(path);
            //}
            //catch (VeilParserException ex)
            //{
            //    error = ex;
            //    errorLocation = ex.Location;
            //}
            //catch (VeilCompilerException ex)
            //{
            //    error = ex;
            //    errorLocation = ex.Node.Location;
            //}

            //return await GetErrorPage(error, errorLocation);
        }

        private HttpResponseMessage GetInternal(string path)
        {
            IPageViewDefinition viewDefinition;

			if (_viewRepository.TryGetFromView(path, out viewDefinition))
                return Get(viewDefinition);

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        private HttpResponseMessage Get(IPageViewDefinition data)
        {
            return View(_viewEngine, data);
        }
    }
}
