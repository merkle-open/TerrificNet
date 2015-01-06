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
				new Tuple<string, string>(null, null),
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
				GenerateCode("Schemas/simpleObjectNoTitle.json");
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
			const string reference = "public class SimpleClass{public string Name{get;set;}}";
			Assert.IsTrue(CompareCode(reference, GenerateCode("Schemas/simpleObject.json")));
		}

		[TestMethod]
		public void TestSimpleObjectAllType()
		{
			const string reference = "public class SimpleClass{public string Name{get;set;} public int Age{get;set;} public bool Male{get;set;}}";
			Assert.IsTrue(CompareCode(reference, GenerateCode("Schemas/simpleObjectAllType.json")));
		}

		[TestMethod]
		public void TestSimpleObjectComplex()
		{
			const string reference = "public class Person{ public string Name{get;set;}}public class SimpleClass{public Person Person{get;set;}} ";
			Assert.IsTrue(CompareCode(reference, GenerateCode("Schemas/simpleObjectComplex.json")));
		}

		[TestMethod]
		public void TestListSimple()
		{
			const string reference = "public class Person{ public IList<string> Names{get;set;}}";
			Assert.IsTrue(CompareCode(reference, GenerateCode("Schemas/listSimple.json")));
		}

		[TestMethod]
		public void TestListComplex()
		{
			const string reference = "public class Names{public string Firstname{get;set;}} public class Person{ public IList<Names> Names{get;set;}}";
			Assert.IsTrue(CompareCode(reference, GenerateCode("Schemas/listComplex.json")));
		}

        [TestMethod]
	    public void TestCompileComplexType()
        {
            var path = "Schemas/listSimple.json";
            var generator = new JsonSchemaCodeGenerator();
            var schema = GetSchema(Path.Combine(TestContext.DeploymentDirectory, path));

            var type = generator.Compile(schema);
            Assert.IsNotNull(type);
        }

		private static bool CompareCode(string original, string generated)
		{
			var originalTree = SyntaxTree.ParseText(original);
			var originalSyntax = originalTree.GetRoot().NormalizeWhitespace();
			var generatedTree = SyntaxTree.ParseText(generated);
			var generatedSyntax = generatedTree.GetRoot().NormalizeWhitespace();

			Console.WriteLine("Original");
			Console.WriteLine("--------");
			Console.WriteLine(originalSyntax.ToFullString());
			Console.WriteLine();

			Console.WriteLine("Generated");
			Console.WriteLine("---------");
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
