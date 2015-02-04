using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using TerrificNet.ViewEngine;

namespace TerrificNet.Controllers
{
    public class ModuleSchemaController : ApiController
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly IModuleSchemaProvider _schemaProvider;

        public ModuleSchemaController(IModuleRepository moduleRepository, IModuleSchemaProvider schemaProvider)
        {
            _moduleRepository = moduleRepository;
            _schemaProvider = schemaProvider;
        }

        [HttpGet]
        public HttpResponseMessage Get(string path)
        {
            ModuleDefinition moduleDefinition;
            if (!_moduleRepository.TryGetModuleDefinitionById(path, out moduleDefinition))
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Template not found");

            var message = new HttpResponseMessage
            {
                Content = new StringContent(_schemaProvider.GetSchemaFromModule(moduleDefinition).ToString())
            };
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return message;            
        }

    }
}