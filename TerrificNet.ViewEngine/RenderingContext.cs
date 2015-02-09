using System.Collections.Generic;
using System.IO;

namespace TerrificNet.ViewEngine
{
    public class RenderingContext
    {
	    private readonly Dictionary<string, object> _data = new Dictionary<string, object>();
        public TextWriter Writer { get; private set; }

        public RenderingContext(TextWriter writer)
        {
            Writer = writer;
        }

		public IDictionary<string, object> Data { get { return _data; } }
    }
}