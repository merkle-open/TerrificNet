using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System.IO;
using System.Threading.Tasks;
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

        public Task<JSchema> GetSchemaFromTemplateAsync(TemplateInfo template)
        {
            var file = _fileSystem.Path.Combine(PathInfo.Create(_config.ViewPath), PathInfo.Create("schemas"), _fileSystem.Path.ChangeExtension(PathInfo.Create(template.Id), "json"));
            if (!_fileSystem.FileExists(file))
                return null;

            return GetSchema(file);
        }

        private async Task<JSchema> GetSchema(PathInfo path)
        {
            string content = null;
            using (var reader = new StreamReader(_fileSystem.OpenRead(path)))
            {
                content = await reader.ReadToEndAsync().ConfigureAwait(false);
            }

            return JsonConvert.DeserializeObject<JSchema>(content);
        }
    }
}
