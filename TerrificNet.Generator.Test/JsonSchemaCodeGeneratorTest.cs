using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Roslyn.Compilers.CSharp;

namespace TerrificNet.Generator.Test
{
	[TestClass]
	public class JsonSchemaCodeGeneratorTest
	{
		public TestContext TestContext { get; set; }

		[TestMethod]
		public void TestClassNameNormalization()
		{
			var inputs = new List<Tuple<string, string>>
			{
				new Tuple<string, string>("class", "Class"),
				new Tuple<string, string>("person info", "PersonInfo"),
				new Tuple<string, string>("person", "Person"),
				new Tuple<string, string>("Person", "Person"),
				new Tuple<string, string>("SimpleClass", "SimpleClass"),
			};

			foreach (var input in inputs)
			{
				var result = RoslynExtension.NormalizeClassName(input.Item1);
				Assert.AreEqual(input.Item2, result);
			}
		}

		[TestMethod]
		public void TestNoTitleSet()
		{
			var status = false;
			try
			{
				var code = GenerateCode("Schemas/simpleObjectNoTitle.json");
			}
			catch (Exception)
			{
				status = true;
			}
			Assert.IsTrue(status, "No exception thrown for unset title");
		}

		[TestMethod]
		public void TestSimpleObject()
		{
			var code = GenerateCode("Schemas/simpleObject.json");
			Assert.IsTrue(!string.IsNullOrEmpty(code), "No code generated");

			const string reference = "class SimpleClass{string Name{get;set;}}";
			Assert.IsTrue(CompareCode(reference, code));
		}

		private static bool CompareCode(string original, string generated)
		{
			var originalSyntax = Syntax.ParseCompilationUnit(original).NormalizeWhitespace();
			var generatedSyntax = Syntax.ParseCompilationUnit(generated).NormalizeWhitespace();

			Console.WriteLine(originalSyntax.ToFullString());
			Console.WriteLine(generatedSyntax.ToFullString());

			return string.Equals(originalSyntax.ToFullString(), generatedSyntax.ToFullString());
		}

		private string GenerateCode(string path)
		{
			var generator = new JsonSchemaCodeGenerator();
			var schema = GetSchema(Path.Combine(TestContext.DeploymentDirectory, path));
			return generator.Generate(schema);
		}

		private static JsonSchema GetSchema(string path)
		{
			string content;
			using (var reader = new StreamReader(path))
			{
				content = reader.ReadToEnd();
			}

			return JsonConvert.DeserializeObject<JsonSchema>(content);
		}
	}
}
