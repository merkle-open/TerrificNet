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
			var inputStream = new StringReader("{{#if name}}Das ist ein Test{{/name}}");
			var input = new AntlrInputStream(inputStream.ReadToEnd());
			var lexer = new MustacheLexer(input);
			ITokenStream tokens = new CommonTokenStream(lexer);
			var parser = new MustacheParser(tokens);
			IParseTree tree = parser.template();

			string[] ruleNames = parser.RuleNames;

			Print(tree, ruleNames, 0);
			//Console.WriteLine(tree.ToStringTree(parser));

			Console.ReadLine();
		}

		private static void Print(ITree node, string[] ruleNames, int level)
		{
			var nodeText = GetNodeText(node, ruleNames);
			Console.WriteLine("{0}{1}", new String(' ', level), nodeText);

			for (var i = 0; i < node.ChildCount; i++)
			{
				Print(node.GetChild(i), ruleNames, level + 1);
			}
		}

		public static string GetNodeText(ITree t, string[] ruleNames)
		{
			if (ruleNames != null)
			{
				if (t is IRuleNode)
				{
					var ruleIndex = ((IRuleNode)t).RuleContext.RuleIndex;
					var ruleName = ruleNames[ruleIndex];
					return ruleName;
				}
				if (t is IErrorNode)
				{
					return t.ToString();
				}
				if (t is ITerminalNode)
				{
					var symbol = ((ITerminalNode)t).Symbol;
					if (symbol != null)
					{
						var s = symbol.Text;
						return s;
					}
				}
			}
			// no recog for rule names
			var payload = t.Payload;
			if (payload is IToken)
			{
				return ((IToken)payload).Text;
			}
			return t.Payload.ToString();
		}
	}
}
