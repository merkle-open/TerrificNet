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
        private readonly IFileSystem _fileSystem;
        private static readonly PathInfo SchemasPathInfo = PathInfo.Create("schemas");
        private readonly PathInfo _viewPathInfo;

        public PhysicalSchemaProvider(ITerrificNetConfig config, IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _viewPathInfo = config.ViewPath;
        }

        public Task<JSchema> GetSchemaFromTemplateAsync(TemplateInfo template)
        {
            var file = _fileSystem.Path.Combine(_viewPathInfo, SchemasPathInfo, _fileSystem.Path.ChangeExtension(PathInfo.Create(template.Id), "json"));
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
