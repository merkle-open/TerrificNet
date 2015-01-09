using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using TerrificNet.Configuration;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.SchemaProviders;
using Newtonsoft.Json.Schema;

namespace TerrificNet.Generator.MSBuild
{
    public class CompileTask : Task
    {
        public override bool Execute()
        {
            Execute(SourcePath, OutputAssembly, RootNamespace);

            return true;
        }

        public static void Execute(string sourcePath, string outputAssembly, string rootNamespace = null)
        {
            ExecuteInternal(sourcePath, (c, s) => CompileToAssembly(c, s, outputAssembly, rootNamespace));
        }

        public static void ExecuteFile(string sourcePath, string fileName, string rootNamespace = null)
        {
            ExecuteInternal(sourcePath, (c, s) => WriteToFile(c, s, fileName, rootNamespace));
        }

        public static string ExecuteString(string sourcePath, string rootNamespace = null)
        {
            using (var stream = new MemoryStream())
            {
                ExecuteInternal(sourcePath, (c, s) => c.WriteTo(s, stream, rootNamespace));

                stream.Seek(0, SeekOrigin.Begin);

                return new StreamReader(stream).ReadToEnd();
            }
        }

        private static void ExecuteInternal(string sourcePath, Action<JsonSchemaCodeGenerator, IEnumerable<JsonSchema>> executeAction)
        {
            var config = new TerrificNetConfig
            {
                BasePath = sourcePath,
                ViewPath = Path.Combine(sourcePath, "views"),
                ModulePath = Path.Combine(sourcePath, "components/modules")
            };

            var schemaProvider = new SchemaMergeProvider(new HandlebarsViewSchemaProvider(),
                new PhysicalSchemaProvider(config));
            var repo = new TerrificTemplateRepository(config);
            var codeGenerator = new JsonSchemaCodeGenerator();

            var schemas = repo.GetAll().Select(t =>
            {
                var schema = schemaProvider.GetSchemaFromTemplate(t);
                if (string.IsNullOrEmpty(schema.Title))
                    schema.Title = t.Id + "Model";

                return schema;
            }).ToList();

            executeAction(codeGenerator, schemas);
        }

        private static void WriteToFile(JsonSchemaCodeGenerator codeGenerator, IEnumerable<JsonSchema> schemas,
            string fileName, string rootNamespace = null)
        {
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                codeGenerator.WriteTo(schemas, stream, rootNamespace);
            }
        }

        private static void CompileToAssembly(JsonSchemaCodeGenerator codeGenerator, IEnumerable<JsonSchema> schemas, string outputAssembly, string rootNamespace = null)
        {
            using (var stream = new FileStream(outputAssembly, FileMode.Create))
            {
                codeGenerator.CompileTo(schemas, stream, rootNamespace);
            }
        }

        [Required]
        public string SourcePath { get; set; }

        [Required]
        public string OutputAssembly { get; set; }

        public string RootNamespace { get; set; }
    }
}
