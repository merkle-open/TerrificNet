using System;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Schema;
using Roslyn.Compilers.CSharp;

namespace TerrificNet.Generator
{
	public class JsonSchemaCodeGenerator : IJsonSchemaCodeGenerator
	{
		public string Generate(JsonSchema schema)
		{
			var root = Syntax.CompilationUnit()
				.WithMembers(Syntax.List(GenerateClass(schema)));

			return root.ToFullString();
		}

		private static MemberDeclarationSyntax GenerateClass(JsonSchema schema)
		{
			if (schema.Type == JsonSchemaType.Object)
			{
				var className = RoslynExtension.NormalizeClassName(schema.Title);

				if (string.IsNullOrEmpty(className))
					throw new Exception("Title not set");

				return Syntax.ClassDeclaration(
					Syntax.Identifier(className))
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


	}

	static class RoslynExtension
	{
		public static SyntaxList<MemberDeclarationSyntax> AddProperties(this SyntaxList<MemberDeclarationSyntax> memberList, JsonSchema schema)
		{
			var result = memberList;
			foreach (var property in schema.Properties)
			{
				var propertyName = NormalizeClassName(property.Key);

				result = result.Add(Syntax.PropertyDeclaration(GetPropertyType(property.Value).WithTrailingTrivia(Syntax.Space), propertyName)
					.WithAccessorList(Syntax.AccessorList(Syntax.List(
						Syntax.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
					.WithSemicolonToken(Syntax.Token(SyntaxKind.SemicolonToken)),
						Syntax.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
					.WithSemicolonToken(Syntax.Token(SyntaxKind.SemicolonToken))
					))));
			}

			return result;
		}

		private static TypeSyntax GetPropertyType(JsonSchema value)
		{
			switch (value.Type)
			{
				case JsonSchemaType.String:
					return Syntax.ParseTypeName("string");
				case JsonSchemaType.Integer:
					return Syntax.ParseTypeName("int");
				case JsonSchemaType.Float:
					return Syntax.ParseTypeName("double");
				case JsonSchemaType.Boolean:
					return Syntax.ParseTypeName("bool");
				case JsonSchemaType.Array:
					return Syntax.ParseTypeName("List");
				default:
					return Syntax.ParseTypeName("object");
			}
		}

		public static string NormalizeClassName(string input)
		{
			return ConvertToPascalCase(input).Replace(" ", "").Trim();
		}

		private static string ConvertToPascalCase(string input)
		{
			return new Regex(@"\p{Lu}\p{Ll}+|\p{Lu}+(?!\p{Ll})|\p{Ll}+|\d+").Replace(input, EvaluatePascal);
		}

		private static string EvaluatePascal(Match match)
		{
			var value = match.Value;
			var valueLength = value.Length;

			if (valueLength == 1)
				return value.ToUpper();
			
			if (valueLength <= 2 && IsWordUpper(value))
				return value;
			
			return value.Substring(0, 1).ToUpper() + value.Substring(1, valueLength - 1).ToLower();
		}

		private static bool IsWordUpper(string word)
		{
			return word.All(c => !Char.IsLower(c));
		}
	}
}
