using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System.IO;
using TerrificNet.ViewEngine.Config;

namespace TerrificNet.ViewEngine.SchemaProviders
{
    public class PhysicalSchemaProvider : ISchemaProvider
    {
        private readonly ITerrificNetConfig _config;
        private readonly IFileSystem _fileSystem;

        public PhysicalSchemaProvider(ITerrificNetConfig config, IFileSystem fileSystem)
        {
            _config = config;
            _fileSystem = fileSystem;
        }

        public JsonSchema GetSchemaFromTemplate(TemplateInfo template)
        {
            var file = Path.Combine(_config.ViewPath, "schemas", Path.ChangeExtension(template.Id, "json"));
            if (!_fileSystem.FileExists(file))
                return null;

            return GetSchema(file);
        }

        private JsonSchema GetSchema(string path)
        {
            string content = null;
            using (var reader = _fileSystem.OpenRead(path))
            {
                content = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<JsonSchema>(content);
        }
    }
}
