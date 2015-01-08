using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TerrificNet.ViewEngine.Config;

namespace TerrificNet.ViewEngine.ModelProviders
{
    public class JsonModelProvider : IModelProvider
    {
        private readonly string _basePath;

        public JsonModelProvider(ITerrificNetConfig config)
        {
            _basePath = config.DataPath;
        }

        public object GetModelForTemplate(TemplateInfo template)
        {
            var filePath = GetPath(template.Id);

	        if (!File.Exists(filePath))
		        return new JObject();

            using (var stream = new StreamReader(filePath))
            {
                var content = stream.ReadToEnd();
                return JsonConvert.DeserializeObject(content);
            }
        }

        public void UpdateModelForTemplate(TemplateInfo template, object content)
        {
            var filePath = GetPath(template.Id);
            using (var stream = new StreamWriter(filePath))
            {
                var value = JsonConvert.SerializeObject(content);
                stream.Write(value);
            }
        }

        private string GetPath(string id)
        {
            return Path.Combine(_basePath, Path.ChangeExtension(id, ".json"));
        }

    }
}