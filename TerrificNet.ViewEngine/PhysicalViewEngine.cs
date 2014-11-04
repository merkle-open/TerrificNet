using System.IO;
using Mustache;

namespace TerrificNet.Controller
{
    public class PhysicalViewEngine : IViewEngine
    {
        private readonly string _basePath;

        public PhysicalViewEngine(string basePath)
        {
            _basePath = basePath;
        }

        public bool TryCreateViewFromPath(string path, out IView view)
        {
            view = null;
            var filePath = Path.Combine(_basePath, path);
            if (!File.Exists(filePath))
                return false;

            var compiler = new FormatCompiler();
            var reader = new StreamReader(filePath);
            var generator = compiler.Compile(reader.ReadToEnd());

            view = new PhysicalView(generator);
            return true;
        }

        private class PhysicalView : IView
        {
            private readonly Generator _generator;

            public PhysicalView(Generator generator)
            {
                _generator = generator;
            }

            public string Render(object model)
            {
                return _generator.Render(model);
            }
        }
    }
}