using System.IO;
using Newtonsoft.Json;
using TerrificNet.ViewEngine.Config;

namespace TerrificNet.ViewEngine.ModelProviders
{
    public class JsonModelProvider : IModelProvider
    {
        private readonly IFileSystem _fileSystem;
        private const string DefaultFilename = "_default.json";
        private readonly string _folderPath;
        private readonly string _basePath;

        public JsonModelProvider(ITerrificNetConfig config, IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _folderPath = _fileSystem.Path.Combine("..", "data");
            _basePath = config.BasePath;
        }

        public object GetDefaultModelForTemplate(TemplateInfo template)
        {
            return GetModelForTemplate(template, DefaultFilename);
        }

        public void UpdateDefaultModelForTemplate(TemplateInfo template, object content)
        {
            var filePath = GetPath(template, DefaultFilename);
            if (!_fileSystem.DirectoryExists(_fileSystem.Path.GetDirectoryName(filePath)))
                _fileSystem.CreateDirectory(_fileSystem.Path.GetDirectoryName(filePath));

            using (var stream = new StreamWriter(_fileSystem.OpenWrite(filePath)))
            {
                var value = JsonConvert.SerializeObject(content);
                stream.Write(value);
            }
        }

        public object GetModelForTemplate(TemplateInfo template, string dataId)
        {
            var filePath = GetPath(template, dataId);

            if (!_fileSystem.FileExists(filePath))
                return null;

            using (var stream = new StreamReader(_fileSystem.OpenRead(filePath)))
            {
                var content = stream.ReadToEnd();
                return JsonConvert.DeserializeObject(content);
            }
        }

        private string GetPath(TemplateInfo templateInfo, string id)
        {
            return _fileSystem.Path.Combine(_basePath, templateInfo.Id, _folderPath, _fileSystem.Path.ChangeExtension(id, ".json"));
        }

    }
}