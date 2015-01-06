using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System.IO;
using TerrificNet.ViewEngine.Config;

namespace TerrificNet.ViewEngine.SchemaProviders
{
    public class PhysicalSchemaProvider : ISchemaProvider
    {
        private readonly ITerrificNetConfig _config;

        public PhysicalSchemaProvider(ITerrificNetConfig config)
        {
            _config = config;
        }

        public JsonSchema GetSchemaFromTemplate(TemplateInfo template)
        {
            var file = Path.Combine(_config.ViewPath, "schemas", Path.ChangeExtension(template.Id, "json"));
            if (!File.Exists(file))
                return null;

            return GetSchema(file);
        }

        private static JsonSchema GetSchema(string path)
        {
            string content = null;
            using (var reader = new StreamReader(path))
            {
                content = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<JsonSchema>(content);
        }
    }
}
