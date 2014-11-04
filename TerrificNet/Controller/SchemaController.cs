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
        public object Get(string path)
        {
            return Json(_schemaProvider.GetSchemaFromPath(path));
        }
    }
}
