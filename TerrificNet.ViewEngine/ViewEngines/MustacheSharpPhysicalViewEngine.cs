using System.IO;

namespace TerrificNet.ViewEngine.ViewEngines
{
    public abstract class PhysicalViewEngineBase : IViewEngine
    {
        private readonly string _basePath;

        protected PhysicalViewEngineBase(string basePath)
        {
            _basePath = basePath;
        }

        public bool TryCreateViewFromPath(string path, out IView view)
        {
            view = null;
            var filePath = Path.Combine(_basePath, path);
            if (!File.Exists(filePath))
                return false;

            using (var reader = new StreamReader(filePath))
            {
                var content = reader.ReadToEnd();

                view = CreateView(content);
                return true;
            }
        }

        protected abstract IView CreateView(string content);
    }
}