using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.Config;
using TerrificNet.ViewEngine.IO;
using TerrificNet.ViewEngine.TemplateHandler.UI;

namespace TerrificNet.Controllers
{
    public class TerrificViewDefinitionRepository
    {
        private readonly IFileSystem _fileSystem;
        private readonly ITemplateRepository _templateRepository;
        private readonly IModelTypeProvider _typeProvider;
        private readonly PathInfo _viewPathInfo;

        public TerrificViewDefinitionRepository(IFileSystem fileSystem, 
            ITerrificNetConfig configuration,
            ITemplateRepository templateRepository,
            IModelTypeProvider typeProvider)
        {
            _fileSystem = fileSystem;
            _templateRepository = templateRepository;
            _typeProvider = typeProvider;
            _viewPathInfo = configuration.ViewPath;
        }

        public bool TryGetFromView(string path, out IPageViewDefinition viewDefinition)
        {
            var fileName = _fileSystem.Path.ChangeExtension(_fileSystem.Path.Combine(_viewPathInfo, PathInfo.Create(path)), "html.json");
            if (_fileSystem.FileExists(fileName))
            {
                if (TryReadPageDefinition(out viewDefinition, fileName)) 
                    return true;
            }

            viewDefinition = null;
            return false;
        }

        public bool TryGetFromViewId(string id, out IPageViewDefinition viewDefinition)
        {
            var fileId = PathInfo.Create(id);
            if (_fileSystem.FileExists(fileId))
            {
                if (TryReadPageDefinition(out viewDefinition, fileId))
                    return true;
            }

            viewDefinition = null;
            return false;
        }

        public async Task<bool> UpdateViewDefinitionForId(string id, IPageViewDefinition viewDefinition)
        {
            var fileId = PathInfo.Create(id);
            if (!_fileSystem.FileExists(fileId)) 
                return false;

            await WritePageDefinition(viewDefinition, fileId).ConfigureAwait(false);
            return true;
        }
		 
        public IEnumerable<IPageViewDefinition> GetAll()
        {
            foreach (var viewPath in _fileSystem.DirectoryGetFiles(_viewPathInfo, "html.json"))
            {
                IPageViewDefinition viewDefinition;
                if (TryReadPageDefinition(out viewDefinition, viewPath))
                    yield return viewDefinition;
            }
        }

        private bool TryReadPageDefinition(out IPageViewDefinition viewDefinition, PathInfo fileName)
        {
            using (var reader = new JsonTextReader(new StreamReader(_fileSystem.OpenRead(fileName))))
            {
                viewDefinition = Deserialize(reader);

                if (viewDefinition == null)
                    return false;

                viewDefinition.Id = fileName.ToString();

                return true;
            }
        }

        public IPageViewDefinition Deserialize(JsonReader reader)
        {
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new ViewDefinitionTypeConverter(_templateRepository, _typeProvider));
            return serializer.Deserialize<IPageViewDefinition>(reader);
        }

        private async Task WritePageDefinition(IPageViewDefinition viewDefinition, PathInfo fileName)
        {
            using (var stream = new StreamWriter(_fileSystem.OpenWrite(fileName)))
            {
                var value = JsonConvert.SerializeObject(viewDefinition, Formatting.Indented);
                await stream.WriteAsync(value).ConfigureAwait(false);
            }
        }
    }
}