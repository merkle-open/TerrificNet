using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Schema;
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
        public async Task<HttpResponseMessage> Get(string path)
        {
            var templateInfo = await _templateRepository.GetTemplateAsync(path).ConfigureAwait(false);
            if (templateInfo != null)
            {
                var schema = await _schemaProvider.GetSchemaFromTemplateAsync(templateInfo).ConfigureAwait(false);
                var message = new HttpResponseMessage
                {
                    Content = new StringContent(schema.ToString())
                };
                message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return message;
            }

            var moduleDefinition = await _moduleRepository.GetModuleDefinitionByIdAsync(path).ConfigureAwait(false);
            if (moduleDefinition != null)
            {
                var schema = await _moduleSchemaProvider.GetSchemaFromModuleAsync(moduleDefinition).ConfigureAwait(false);
                var message = new HttpResponseMessage
                {
                    Content = new StringContent(schema.ToString())
                };
                message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return message;

            }

            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Template not found");
        }
    }
}
