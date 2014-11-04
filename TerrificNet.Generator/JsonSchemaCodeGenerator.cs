using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema;
using Roslyn.Compilers.CSharp;

namespace TerrificNet.Generator
{
	public class JsonSchemaCodeGenerator : IJsonSchemaCodeGenerator
	{
		public string Generate()
		{
			var schema = new JsonSchemaGenerator().Generate(typeof(TestClass));

			var root = Syntax.CompilationUnit()
				.WithMembers(Syntax.List(GenerateClass(schema)));

			return root.ToString();
		}

		private static MemberDeclarationSyntax GenerateClass(JsonSchema schema)
		{
			if (schema.Type == JsonSchemaType.Object)
			{
				return Syntax.ClassDeclaration(
					Syntax.Identifier("Class1"))
					.WithKeyword(
						Syntax.Token(SyntaxKind.ClassKeyword, Syntax.TriviaList(Syntax.Space)))
					.WithOpenBraceToken(
						Syntax.Token(SyntaxKind.OpenBraceToken))
					.WithMembers(new SyntaxList<MemberDeclarationSyntax>().AddProperties(schema))
					.WithCloseBraceToken(
						Syntax.Token(SyntaxKind.CloseBraceToken));
			}

			return null;
		}

		class TestClass
		{
			public string Name { get; set; }
			public TestClass2 Complex { get; set; }
			public string Name2 { get; set; }
		}

		class TestClass2
		{
			public string Test { get; set; }
		}
	}

	public static class RoslynExtension
	{
		public static SyntaxList<MemberDeclarationSyntax> AddProperties(this SyntaxList<MemberDeclarationSyntax> memberList, JsonSchema schema)
		{
			var result = memberList;
			foreach (var property in schema.Properties)
			{
				result = result.Add(Syntax.PropertyDeclaration(Syntax.ParseTypeName("string").WithTrailingTrivia(Syntax.Space), property.Key)
					.WithAccessorList(Syntax.AccessorList(Syntax.List(
						Syntax.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
					.WithSemicolonToken(Syntax.Token(SyntaxKind.SemicolonToken)),
						Syntax.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
					.WithSemicolonToken(Syntax.Token(SyntaxKind.SemicolonToken))
					))));
			}

			return result;
		}
	}
}
