using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using TerrificNet.ViewEngine;

namespace TerrificNet.Controller
{
    public class SchemaController : ApiController
    {
        private readonly ISchemaProvider _schemaProvider;

        public SchemaController(ISchemaProvider schemaProvider)
        {
            _schemaProvider = schemaProvider;
        }

        [Route("schema/{*path}")]
        [HttpGet]
        public HttpResponseMessage Get(string path)
        {
            var message = new HttpResponseMessage
            {
                Content = new StringContent(_schemaProvider.GetSchemaFromPath(path).ToString())
            };
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return message;
        }
    }
}
