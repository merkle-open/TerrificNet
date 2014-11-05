using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace TerrificNet.Mustache.Antlr
{
	class Program
	{
		static void Main(string[] args)
		{
			var inputStream = new StringReader("{{#name}}{{/name}}");
			AntlrInputStream input = new AntlrInputStream(inputStream.ReadToEnd());
			MustacheLexer lexer = new MustacheLexer(input);
			ITokenStream tokens = new CommonTokenStream(lexer);
			MustacheParser parser = new MustacheParser(tokens);
			IParseTree tree = parser.mustache();
			Console.WriteLine(tree.ToStringTree(parser));

			Console.ReadLine();
		}
	}
}
