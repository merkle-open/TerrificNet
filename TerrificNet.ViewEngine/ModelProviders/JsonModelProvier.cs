using System.IO;
using Newtonsoft.Json;

namespace TerrificNet.ViewEngine.ModelProviders
{
    public class JsonModelProvier : IModelProvider
    {
        private readonly string _basePath;

        public JsonModelProvier(string basePath)
        {
            _basePath = basePath;
        }

        public object GetModelFromPath(string path)
        {
            var filePath = Path.Combine(_basePath, Path.ChangeExtension(path, ".json"));

            using (var stream = new StreamReader(filePath))
            {
                var content = stream.ReadToEnd();
                return JsonConvert.DeserializeObject(content);
            }
        }
    }
}