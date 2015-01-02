using System.IO;
using Newtonsoft.Json;
using TerrificNet.ViewEngine.Config;

namespace TerrificNet.ViewEngine.ModelProviders
{
    public class JsonModelProvier : IModelProvider
    {
        private readonly string _basePath;

        public JsonModelProvier(ITerrificNetConfig config)
        {
            _basePath = config.DataPath;
        }

        public object GetModelForTemplate(string template)
        {
            var filePath = GetPath(template);

	        if (!File.Exists(filePath))
		        return null;

            using (var stream = new StreamReader(filePath))
            {
                var content = stream.ReadToEnd();
                return JsonConvert.DeserializeObject(content);
            }
        }

        public void UpdateModelForTemplate(string template, object content)
        {
            var filePath = GetPath(template);
            using (var stream = new StreamWriter(filePath))
            {
                var value = JsonConvert.SerializeObject(content);
                stream.Write(value);
            }
        }

        private string GetPath(string path)
        {
            return Path.Combine(_basePath, Path.ChangeExtension(path, ".json"));
        }

    }
}