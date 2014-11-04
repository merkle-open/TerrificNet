using System.IO;
using Mustache;

namespace TerrificNet.ViewEngine
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

    public class MustacheSharpPhysicalViewEngine : PhysicalViewEngineBase
    {
        public MustacheSharpPhysicalViewEngine(string basePath) : base(basePath)
        {
        }

        protected override IView CreateView(string content)
        {
            var compiler = new FormatCompiler();
            var generator = compiler.Compile(content);

            return new MustacheView(generator);
        }

        private class MustacheView : IView
        {
            private readonly Generator _generator;

            public MustacheView(Generator generator)
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