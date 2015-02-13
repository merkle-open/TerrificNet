using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using TerrificNet.ViewEngine;

namespace TerrificNet.Controllers
{
    public class SchemaController : ApiController
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly ISchemaProvider _schemaProvider;
        private readonly IModuleSchemaProvider _moduleSchemaProvider;

        public SchemaController(IModuleRepository moduleRepository,
            ITemplateRepository templateRepository, 
            ISchemaProvider schemaProvider,
            IModuleSchemaProvider moduleSchemaProvider)
        {
            _moduleRepository = moduleRepository;
            _templateRepository = templateRepository;
            _schemaProvider = schemaProvider;
            _moduleSchemaProvider = moduleSchemaProvider;
        }

        [HttpGet]
        public HttpResponseMessage Get(string path)
        {
            TemplateInfo templateInfo;
            if (_templateRepository.TryGetTemplate(path, out templateInfo))
            {
                var message = new HttpResponseMessage
                {
                    Content = new StringContent(_schemaProvider.GetSchemaFromTemplate(templateInfo).ToString())
                };
                message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return message;
            }

            ModuleDefinition moduleDefinition;
            if (_moduleRepository.TryGetModuleDefinitionById(path, out moduleDefinition))
            {
                var message = new HttpResponseMessage
                {
                    Content = new StringContent(_moduleSchemaProvider.GetSchemaFromModule(moduleDefinition).ToString())
                };
                message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return message;

            }

            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Template not found");
        }
    }
}
