using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrificNet.ViewEngine.Config;

namespace TerrificNet.ViewEngine.SchemaProviders
{
    public class BaseSchemaProvider : ISchemaProvider
    {
        private readonly ITerrificNetConfig _config;

        public BaseSchemaProvider(ITerrificNetConfig config)
        {
            _config = config;
        }

        public JsonSchema GetSchemaFromPath(string path)
        {
            var file = Path.Combine(_config.ViewPath, "schemas", Path.ChangeExtension(path, "json"));
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
