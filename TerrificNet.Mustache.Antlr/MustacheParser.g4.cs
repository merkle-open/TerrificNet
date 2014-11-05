using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;

namespace TerrificNet.Mustache.Antlr
{
	partial class MustacheParser
	{
		void setStart(String start)
		{
		}

		void setEnd(String end)
		{
		}

		private String join(IEnumerable<IToken> tokens)
		{
			var text = new StringBuilder();
			foreach (var token in tokens)
			{
				text.Append(token.Text);
			}
			return text.ToString();
		}
	}
}
