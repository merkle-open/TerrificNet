using Newtonsoft.Json.Schema;

namespace TerrificNet.ViewEngine.SchemaProviders
{
    public class NustacheViewSchemaProvider : ISchemaProvider
    {
        private readonly string _basePath;

        public NustacheViewSchemaProvider(string basePath)
        {
            _basePath = basePath;
        }

        public JsonSchema GetSchemaFromPath(string path)
        {
            var schema = new JsonSchema();

            return schema;
        }
    }
}
