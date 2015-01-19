using System.IO;

namespace TerrificNet.ViewEngine
{
    public class RenderingContext
    {
        public TextWriter Writer { get; private set; }

        public RenderingContext(TextWriter writer)
        {
            Writer = writer;
        }
    }
}