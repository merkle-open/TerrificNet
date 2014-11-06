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
			var template = "Hallo {{#if age}}Das ist {{name}} ein Test{{/if}}";
			var c = new MustacheTemplateEngine().Compile(template);

			Console.WriteLine("-------------");
			Console.WriteLine(c(new { name = "Hans", age = 20 }));

			Console.ReadLine();
		}
	}
}
