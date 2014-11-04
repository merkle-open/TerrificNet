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
            var filePath = GetPath(path);
            using (var stream = new StreamReader(filePath))
            {
                var content = stream.ReadToEnd();
                return JsonConvert.DeserializeObject(content);
            }
        }

        public void UpdateModelFromPath(string path, object content)
        {
            var filePath = GetPath(path);
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