using System.IO;
using Newtonsoft.Json.Schema;
using TerrificNet.ViewEngine.Schema;

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
            var extractor = new SchemaExtractor();
            return extractor.Run(Path.Combine(_basePath, path));
        }
    }
}
