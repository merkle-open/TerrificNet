using System.IO;
using System.Text;
using Nustache.Core;

namespace TerrificNet.ViewEngine.ViewEngines
{
    public class NustachePhysicalViewEngine : PhysicalViewEngineBase
    {
        public NustachePhysicalViewEngine(string basePath) : base(basePath)
        {
        }

        protected override IView CreateView(string content)
        {
            var template = new Nustache.Core.Template();
            template.Load(new StringReader(content));

            return new NustacheView(template);
        }

        private class NustacheView : IView
        {
            private readonly Template _template;

            public NustacheView(Template template)
            {
                _template = template;
            }

            public string Render(object model)
            {
                var builder = new StringBuilder();
                using (var writer = new StringWriter(builder))
                {
                    _template.Render(model, writer, null);
                }

                return builder.ToString();
            }
        }
    }
}
