using System.IO;
using Newtonsoft.Json;

namespace TerrificNet.ViewEngine.ModelProviders
{
    public class JsonModelProvider : IModelProvider
    {
        private readonly IFileSystem _fileSystem;
        private const string DefaultFilename = "_default.json";
        private const string FolderPath = "../data";

        public JsonModelProvider(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
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
            return _fileSystem.Path.Combine(templateInfo.Id, FolderPath, _fileSystem.Path.ChangeExtension(id, ".json"));
        }

    }
}