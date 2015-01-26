using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Schema;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.Config;
using TerrificNet.ViewEngine.SchemaProviders;

namespace TerrificNet.Generator
{
    public class GeneratorUtility
    {
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
            var config = ConfigurationLoader.LoadTerrificConfiguration(sourcePath, new FileSystem());

            var fileSystem = new FileSystem();
            var schemaProvider = new SchemaMergeProvider(new HandlebarsViewSchemaProvider(),
                new PhysicalSchemaProvider(config, fileSystem));
            var repo = new TerrificTemplateRepository(config, fileSystem);
            var codeGenerator = new JsonSchemaCodeGenerator();

            var schemas = repo.GetAll().Select(schemaProvider.GetSchemaFromTemplate).ToList();

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
    }
}