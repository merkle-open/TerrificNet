using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TerrificNet.ViewEngine.IO;

namespace TerrificNet.ViewEngine.ModelProviders
{
    public class JsonModelProvider : IModelProvider
    {
        private readonly IFileSystem _fileSystem;
        private static readonly PathInfo DefaultFilename = PathInfo.Create("_default.json");
        private static readonly PathInfo FolderPath = PathInfo.Create("../data");
        private static readonly PathInfo ModuleFolderPath = PathInfo.Create("data");

        public JsonModelProvider(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public Task<object> GetDefaultModelForTemplateAsync(TemplateInfo template)
        {
            var filePath = GetPath(template, DefaultFilename);
            return GetModelFromPathAsync(filePath);
        }

        public Task UpdateDefaultModelForTemplateAsync(TemplateInfo template, object content)
        {
            var filePath = GetPath(template, DefaultFilename);
            if (!_fileSystem.DirectoryExists(_fileSystem.Path.GetDirectoryName(filePath)))
                _fileSystem.CreateDirectory(_fileSystem.Path.GetDirectoryName(filePath));

            return Update(content, filePath);
        }

	    public Task<object> GetModelForTemplateAsync(TemplateInfo template, string dataId)
        {
            var filePath = GetPath(template, PathInfo.Create(dataId));
            return GetModelFromPathAsync(filePath);
        }

        public Task<object> GetModelForModuleAsync(ModuleDefinition moduleDefinition, string dataId)
        {
            var filePath = GetPath(moduleDefinition, dataId != null ? PathInfo.Create(dataId) : DefaultFilename);
            return GetModelFromPathAsync(filePath);
        }

	    public Task UpdateModelForModuleAsync(ModuleDefinition moduleDefinition, string dataId, object content)
	    {
            var filePath = GetPath(moduleDefinition, dataId != null ? PathInfo.Create(dataId) : DefaultFilename);
			if (!_fileSystem.DirectoryExists(_fileSystem.Path.GetDirectoryName(filePath)))
				_fileSystem.CreateDirectory(_fileSystem.Path.GetDirectoryName(filePath));

		    return Update(content, filePath);
	    }

	    public IEnumerable<string> GetDataVariations(ModuleDefinition moduleDefinition)
	    {
		    var directory = _fileSystem.Path.Combine(PathInfo.Create(moduleDefinition.Id), ModuleFolderPath);
		    if (!_fileSystem.DirectoryExists(directory))
			    return Enumerable.Empty<string>();

		    return _fileSystem.DirectoryGetFiles(directory, "json")
				.Select(f => _fileSystem.Path.GetFileNameWithoutExtension(f).ToString());
	    }

	    private async Task<object> GetModelFromPathAsync(PathInfo filePath)
        {
            if (!_fileSystem.FileExists(filePath))
                return null;

            using (var stream = new StreamReader(_fileSystem.OpenRead(filePath)))
            {
                var content = await stream.ReadToEndAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject(content);
            }
        }

		private Task Update(object content, PathInfo filePath)
		{
			using (var stream = new StreamWriter(_fileSystem.OpenWrite(filePath)))
			{
				var value = JsonConvert.SerializeObject(content);
				return stream.WriteAsync(value);
			}
		}

        private PathInfo GetPath(ModuleDefinition moduleDefinition, PathInfo dataId)
        {
            return _fileSystem.Path.Combine(PathInfo.Create(moduleDefinition.Id), ModuleFolderPath,
                _fileSystem.Path.ChangeExtension(dataId, ".json"));
        }

        private PathInfo GetPath(TemplateInfo templateInfo, PathInfo id)
        {
            return _fileSystem.Path.Combine(PathInfo.Create(templateInfo.Id), FolderPath, _fileSystem.Path.ChangeExtension(id, ".json"));
        }

    }
}