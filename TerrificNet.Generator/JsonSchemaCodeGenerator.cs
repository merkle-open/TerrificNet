using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Schema;
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;

namespace TerrificNet.Generator
{
	public class JsonSchemaCodeGenerator : IJsonSchemaCodeGenerator
	{
		public string Generate(JsonSchema schema)
		{
		    var root = GetSyntax(schema);
		    return root.NormalizeWhitespace().ToFullString();
		}

	    private static CompilationUnitSyntax GetSyntax(JsonSchema schema)
	    {
	        var typeContext = new Dictionary<string, MemberDeclarationSyntax>();

	        RoslynExtension.GenerateClass(schema, typeContext, string.Empty);

	        var root = Syntax.CompilationUnit()
	            .WithMembers(Syntax.List(typeContext.Values.ToArray()));
	        return root;
	    }

	    public Type Compile(JsonSchema schema)
        {
            var output = GetSyntax(schema);

            var syntaxTree = SyntaxTree.Create(output);

            var compilation = Compilation.Create("test", references: new[]
                {
                    MetadataReference.CreateAssemblyReference(typeof(object).Assembly.FullName),
                    MetadataReference.CreateAssemblyReference(typeof(Enumerable).Assembly.FullName),
                    MetadataReference.CreateAssemblyReference(typeof(List<>).Assembly.FullName),
                },
                syntaxTrees: new[] { syntaxTree },
                options: new CompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            //var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("DynamicModule"),
            //    AssemblyBuilderAccess.RunAndCollect);

            //var moduleBuilder = assemblyBuilder.DefineDynamicModule("Module");

            using (var stream = new MemoryStream())
            {
                var result = compilation.Emit(stream);
                if (result.Success)
                {
                    var assembly = Assembly.Load(stream.ToReadOnlyArray().ToArray());
                    return assembly.GetTypes().First();
                }
            }

            return null;
        }
	}

	static class RoslynExtension
	{
		public static SyntaxList<MemberDeclarationSyntax> AddProperties(this SyntaxList<MemberDeclarationSyntax> memberList, JsonSchema schema, Dictionary<string, MemberDeclarationSyntax> typeContext)
		{
			var result = memberList;
			foreach (var property in schema.Properties)
			{
				var propertyName = NormalizeClassName(property.Key);

				result = result.Add(Syntax.PropertyDeclaration(GetPropertyType(property.Value, typeContext, propertyName).WithTrailingTrivia(Syntax.Space), propertyName)
					.WithAccessorList(Syntax.AccessorList(Syntax.List(
						Syntax.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
					.WithSemicolonToken(Syntax.Token(SyntaxKind.SemicolonToken)),
						Syntax.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
					.WithSemicolonToken(Syntax.Token(SyntaxKind.SemicolonToken))
					))));
			}

			return result;
		}

		private static TypeSyntax GetPropertyType(JsonSchema value, Dictionary<string, MemberDeclarationSyntax> typeContext, string propertyName)
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
					return Syntax.IdentifierName(string.Format("List<{0}>", NormalizeClassName(value.Title)));
				case JsonSchemaType.Object:
					GenerateClass(value, typeContext, propertyName);
					return Syntax.IdentifierName(NormalizeClassName(propertyName));
				default:
					return Syntax.ParseTypeName("object");
			}
		}

		public static void GenerateClass(JsonSchema schema, Dictionary<string, MemberDeclarationSyntax> typeContext, string propertyName)
		{
			if (schema.Type == JsonSchemaType.Object)
			{
				var className = NormalizeClassName(schema.Title);
				if (string.IsNullOrEmpty(className))
					className = propertyName;

				if (typeContext.ContainsKey(className))
					return;

				if (string.IsNullOrEmpty(className))
					throw new Exception("Title not set");

				var classDeclaration= Syntax.ClassDeclaration(
					Syntax.Identifier(className))
					.WithKeyword(
						Syntax.Token(SyntaxKind.ClassKeyword, Syntax.TriviaList(Syntax.Space)))
					.WithOpenBraceToken(
						Syntax.Token(SyntaxKind.OpenBraceToken))
					.WithMembers(new SyntaxList<MemberDeclarationSyntax>().AddProperties(schema, typeContext))
					.WithCloseBraceToken(
						Syntax.Token(SyntaxKind.CloseBraceToken));

				typeContext.Add(className, classDeclaration);
			}
		}

		public static string NormalizeClassName(string input)
		{
			if (string.IsNullOrEmpty(input))
				return input;

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
