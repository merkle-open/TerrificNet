using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Schema;
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
        public async Task<HttpResponseMessage> Get(string path)
        {
            var moduleDefinition = await _moduleRepository.GetModuleDefinitionByIdAsync(path).ConfigureAwait(false);
            if (moduleDefinition == null)
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Template not found");

            var schema = await _schemaProvider.GetSchemaFromModuleAsync(moduleDefinition).ConfigureAwait(false);
            var message = new HttpResponseMessage
            {
                Content = new StringContent(schema.ToString())
            };
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return message;            
        }

    }
}