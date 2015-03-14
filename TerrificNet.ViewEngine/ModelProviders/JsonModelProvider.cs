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
        private const string DefaultFilename = "_default.json";
        private const string FolderPath = "../data";
        private const string ModuleFolerPath = "data";

        public JsonModelProvider(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public Task<object> GetDefaultModelForTemplateAsync(TemplateInfo template)
        {
            return GetModelForTemplateAsync(template, DefaultFilename);
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
            var filePath = GetPath(template, dataId);
            return GetModelFromPathAsync(filePath);
        }

        public Task<object> GetModelForModuleAsync(ModuleDefinition moduleDefinition, string dataId)
        {
            var filePath = GetPath(moduleDefinition, dataId ?? DefaultFilename);
            return GetModelFromPathAsync(filePath);
        }

	    public Task UpdateModelForModuleAsync(ModuleDefinition moduleDefinition, string dataId, object content)
	    {
			var filePath = GetPath(moduleDefinition, dataId ?? DefaultFilename);
			if (!_fileSystem.DirectoryExists(_fileSystem.Path.GetDirectoryName(filePath)))
				_fileSystem.CreateDirectory(_fileSystem.Path.GetDirectoryName(filePath));

		    return Update(content, filePath);
	    }

	    public IEnumerable<string> GetDataVariations(ModuleDefinition moduleDefinition)
	    {
		    var directory = _fileSystem.Path.Combine(moduleDefinition.Id, ModuleFolerPath);
		    if (!_fileSystem.DirectoryExists(directory))
			    return Enumerable.Empty<string>();

		    return _fileSystem.DirectoryGetFiles(directory, "json")
				.Select(f => _fileSystem.Path.GetFileNameWithoutExtension(f));
	    }

	    private async Task<object> GetModelFromPathAsync(string filePath)
        {
            if (!_fileSystem.FileExists(filePath))
                return null;

            using (var stream = new StreamReader(_fileSystem.OpenRead(filePath)))
            {
                var content = await stream.ReadToEndAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject(content);
            }
        }

		private Task Update(object content, string filePath)
		{
			using (var stream = new StreamWriter(_fileSystem.OpenWrite(filePath)))
			{
				var value = JsonConvert.SerializeObject(content);
				return stream.WriteAsync(value);
			}
		}

        private string GetPath(ModuleDefinition moduleDefinition, string dataId)
        {
            return _fileSystem.Path.Combine(moduleDefinition.Id, ModuleFolerPath,
                _fileSystem.Path.ChangeExtension(dataId, ".json"));
        }

        private string GetPath(TemplateInfo templateInfo, string id)
        {
            return _fileSystem.Path.Combine(templateInfo.Id, FolderPath, _fileSystem.Path.ChangeExtension(id, ".json"));
        }

    }
}