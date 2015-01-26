using System.IO;
using Newtonsoft.Json;
using TerrificNet.ViewEngine.Config;

namespace TerrificNet.ViewEngine.ModelProviders
{
    public class JsonModelProvider : IModelProvider
    {
        private const string DefaultFilename = "_default.json";
        private const string Foldername = "..\\data";
        private readonly string _basePath;

        public JsonModelProvider(ITerrificNetConfig config)
        {
            _basePath = config.BasePath;
        }

        public object GetDefaultModelForTemplate(TemplateInfo template)
        {
            return GetModelForTemplate(template, DefaultFilename);
        }

        public void UpdateDefaultModelForTemplate(TemplateInfo template, object content)
        {
            var filePath = GetPath(template, DefaultFilename);
            using (var stream = new StreamWriter(filePath))
            {
                var value = JsonConvert.SerializeObject(content);
                stream.Write(value);
            }
        }

        public object GetModelForTemplate(TemplateInfo template, string dataId)
        {
            var filePath = GetPath(template, dataId);

            if (!File.Exists(filePath))
                return null;

            using (var stream = new StreamReader(filePath))
            {
                var content = stream.ReadToEnd();
                return JsonConvert.DeserializeObject(content);
            }
        }

        private string GetPath(TemplateInfo templateInfo, string id)
        {
            return Path.Combine(_basePath, templateInfo.Id, Foldername, Path.ChangeExtension(id, ".json"));
        }

    }
}