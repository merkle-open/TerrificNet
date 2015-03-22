using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Schema;
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;
using TerrificNet.ViewEngine;

namespace TerrificNet.Generator
{
    public class JsonSchemaCodeGenerator : IJsonSchemaCodeGenerator
    {
        private readonly INamingRule _namingRule;

        public JsonSchemaCodeGenerator() : this(new NamingRule())
        {
        }

        public JsonSchemaCodeGenerator(INamingRule namingRule)
        {
            _namingRule = namingRule;
        }

        public string Generate(JSchema schema)
        {
            var root = GetSyntax(schema);
            return root.NormalizeWhitespace().ToFullString();
        }

        private CompilationUnitSyntax GetSyntax(JSchema schema)
        {
            var typeContext = new Dictionary<string, MemberDeclarationSyntax>();

            RoslynExtension.GenerateClass(schema, typeContext, string.Empty, _namingRule);

            var root = Syntax.CompilationUnit()
                .WithMembers(Syntax.NamespaceDeclaration(Syntax.ParseName(_namingRule.GetNamespaceName(schema))).WithMembers(Syntax.List(typeContext.Values.ToArray())));
            return root;
        }

        public Type Compile(JSchema schema)
        {
            if (schema.Properties == null || schema.Properties.Count == 0)
                return typeof (object);

            var schemas = new[] { schema };
            using (var stream = new MemoryStream())
            {
                var result = CompileToInternal(schemas, stream);
                if (result.Success)
                {
                    var typeName = _namingRule.GetClassName(schema, null);
                    var assembly = Assembly.Load(stream.ToReadOnlyArray().ToArray());
                    return assembly.GetTypes().First(t => t.Name == typeName);
                }
            }

            return null;
        }

        public void WriteTo(IEnumerable<JSchema> schemas, Stream stream, string rootNamespace = null)
        {
            var syntaxTree = GetSyntaxTree(schemas, rootNamespace);
            var text = syntaxTree.GetText();
            var streamWriter = new StreamWriter(stream);
            
            text.Write(streamWriter);
            streamWriter.Flush();            
        }

        public void CompileTo(IEnumerable<JSchema> schemas, Stream stream, string rootNamespace = null)
        {
            CompileToInternal(schemas, stream, rootNamespace);
        }

        private EmitResult CompileToInternal(IEnumerable<JSchema> schemas, Stream stream, string rootNamespace = null)
        {
            var syntaxTree = GetSyntaxTree(schemas, rootNamespace);

            var compilation = Compilation.Create("test", new CompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(MetadataReference.CreateAssemblyReference("mscorlib"))
                .AddReferences(MetadataReference.CreateAssemblyReference("System"))
                .AddReferences(MetadataReference.CreateAssemblyReference("System.Core"))
                .AddSyntaxTrees(syntaxTree);

            var result = compilation.Emit(stream);
            return result;
        }

        private SyntaxTree GetSyntaxTree(IEnumerable<JSchema> schemas, string rootNamespace = null)
        {
            var syntaxTree = SyntaxTree.Create(
                Syntax.CompilationUnit()
                    .WithMembers(GetRootNamespace(schemas, rootNamespace))
                    .NormalizeWhitespace());

            return syntaxTree;
        }

        private SyntaxList<MemberDeclarationSyntax> GetRootNamespace(IEnumerable<JSchema> schemas, string rootNamespace)
        {
            var list = Syntax.List(schemas.SelectMany(s => GetSyntax(s).Members));
            if (!string.IsNullOrEmpty(rootNamespace))
                list = Syntax.NamespaceDeclaration(Syntax.ParseName(rootNamespace)).WithMembers(list);

            return list;
        }
    }

    static class RoslynExtension
    {
        public static SyntaxList<MemberDeclarationSyntax> AddProperties(this SyntaxList<MemberDeclarationSyntax> memberList, JSchema schema, Dictionary<string, MemberDeclarationSyntax> typeContext, INamingRule namingRule)
        {
            var result = memberList;
            if (schema.Properties != null)
            {
                foreach (var property in schema.Properties)
                {
                    var propertyName = namingRule.GetPropertyName(property.Key);

                    result =
                        result.Add(Syntax.PropertyDeclaration(
                            GetPropertyType(property.Value, typeContext, propertyName, namingRule).WithTrailingTrivia(Syntax.Space),
                            propertyName)
                            .WithModifiers(new SyntaxTokenList().Add(Syntax.Token(SyntaxKind.PublicKeyword)))
                            .WithAccessorList(Syntax.AccessorList(Syntax.List(
                                Syntax.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                    .WithSemicolonToken(Syntax.Token(SyntaxKind.SemicolonToken)),
                                Syntax.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                                    .WithSemicolonToken(Syntax.Token(SyntaxKind.SemicolonToken))
                                ))));
                }
            }

            return result;
        }

        private static TypeSyntax GetPropertyType(JSchema value, Dictionary<string, MemberDeclarationSyntax> typeContext, string propertyName, INamingRule namingRule)
        {
            switch (value.Type)
            {
                case JSchemaType.String:
                    return Syntax.ParseTypeName("string");
                case JSchemaType.Integer:
                    return Syntax.ParseTypeName("int");
                case JSchemaType.Number:
                    return Syntax.ParseTypeName("double");
                case JSchemaType.Boolean:
                    return Syntax.ParseTypeName("bool");
                case JSchemaType.Array:
                    var valueType = value.Items.FirstOrDefault();
                    var name = namingRule.GetClassNameFromArrayItem(value, propertyName);
                    var genericType = GetPropertyType(valueType, typeContext, name, namingRule);
                    return Syntax.QualifiedName(GetQualifiedName("System", "Collections", "Generic"), Syntax.GenericName(Syntax.Identifier("IList"), Syntax.TypeArgumentList(Syntax.SeparatedList(genericType))));
                case JSchemaType.Object:
                    var className = GenerateClass(value, typeContext, propertyName, namingRule);
                    return Syntax.IdentifierName(className);
                default:
                    return Syntax.ParseTypeName("object");
            }
        }

        // TODO: Remove hack
        private static QualifiedNameSyntax GetQualifiedName(params string[] parts)
        {
            var syntax = Syntax.QualifiedName(Syntax.IdentifierName(parts[0]), Syntax.IdentifierName(parts[1]));
            return Syntax.QualifiedName(syntax, Syntax.IdentifierName(parts[2]));
        }

        public static string GenerateClass(JSchema schema, Dictionary<string, MemberDeclarationSyntax> typeContext, string propertyName, INamingRule namingRule)
        {
            if (schema.Type == JSchemaType.Object)
            {
                var className = namingRule.GetClassName(schema, propertyName);

                if (typeContext.ContainsKey(className))
                    return className;

                if (string.IsNullOrEmpty(className))
                    throw new Exception("Title not set");

                var classDeclaration = Syntax.ClassDeclaration(
                    Syntax.Identifier(className))
                    .WithModifiers(new SyntaxTokenList().Add(Syntax.Token(SyntaxKind.PublicKeyword)))
                    .WithKeyword(
                        Syntax.Token(SyntaxKind.ClassKeyword, Syntax.TriviaList(Syntax.Space)))
                    .WithOpenBraceToken(
                        Syntax.Token(SyntaxKind.OpenBraceToken))
                    .WithMembers(new SyntaxList<MemberDeclarationSyntax>().AddProperties(schema, typeContext, namingRule))
                    .WithCloseBraceToken(
                        Syntax.Token(SyntaxKind.CloseBraceToken));

                typeContext.Add(className, classDeclaration);
                return className;
            }
            return null;
        }
    }
}
