using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System.IO;
using TerrificNet.ViewEngine.Config;
using TerrificNet.ViewEngine.IO;

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

        public JSchema GetSchemaFromTemplate(TemplateInfo template)
        {
            var file = Path.Combine(_config.ViewPath, "schemas", Path.ChangeExtension(template.Id, "json"));
            if (!_fileSystem.FileExists(file))
                return null;

            return GetSchema(file);
        }

        private JSchema GetSchema(string path)
        {
            string content = null;
            using (var reader = new StreamReader(_fileSystem.OpenRead(path)))
            {
                content = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<JSchema>(content);
        }
    }
}
