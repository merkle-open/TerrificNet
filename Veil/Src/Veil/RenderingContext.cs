using System.Collections.Generic;
using System.IO;

namespace Veil
{
    public class RenderingContext
    {
	    private readonly Dictionary<string, object> _data = new Dictionary<string, object>();
        public TextWriter Writer { get; private set; }

	    public RenderingContext(TextWriter writer, RenderingContext parentContext)
	    {
			Writer = writer;

			if (parentContext != null)
			{
				foreach (var dataEntry in parentContext.Data)
					this.Data.Add(dataEntry);
			}
	    }

        public RenderingContext(TextWriter writer) : this(writer, null)
        {
        }

		public IDictionary<string, object> Data { get { return _data; } }
    }
}