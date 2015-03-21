using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.Config;
using TerrificNet.ViewEngine.IO;
using TerrificNet.ViewEngine.SchemaProviders;

namespace TerrificNet.Generator
{
    public class GeneratorUtility
    {
        public static Task ExecuteAsync(string sourcePath, string outputAssembly, string rootNamespace = null)
        {
            return ExecuteInternal(sourcePath, (c, s) => CompileToAssembly(c, s, outputAssembly, rootNamespace));
        }

        public static Task ExecuteFileAsync(string sourcePath, string fileName, string rootNamespace = null)
        {
            return ExecuteInternal(sourcePath, (c, s) => WriteToFile(c, s, fileName, rootNamespace));
        }

        public static string ExecuteString(string sourcePath, string rootNamespace = null)
        {
            return ExecuteStringAsync(sourcePath, rootNamespace).Result;
        }

        public async static Task<string> ExecuteStringAsync(string sourcePath, string rootNamespace = null)
        {
            using (var stream = new MemoryStream())
            {
                await ExecuteInternal(sourcePath, (c, s) => c.WriteTo(s, stream, rootNamespace)).ConfigureAwait(false);

                stream.Seek(0, SeekOrigin.Begin);

                return await new StreamReader(stream).ReadToEndAsync().ConfigureAwait(false);
            }
        }

        private static async Task ExecuteInternal(string sourcePath, Action<JsonSchemaCodeGenerator, IEnumerable<JSchema>> executeAction)
        {
            var fileSystem = new FileSystem(sourcePath);
            var config = ConfigurationLoader.LoadTerrificConfiguration(string.Empty, fileSystem);

            var schemaProvider = new SchemaMergeProvider(new HandlebarsViewSchemaProvider(null, null),
                new PhysicalSchemaProvider(config, fileSystem));
            var repo = new TerrificTemplateRepository(fileSystem);
            var codeGenerator = new JsonSchemaCodeGenerator();

            var schemas = repo.GetAll().Select(schemaProvider.GetSchemaFromTemplateAsync).ToList();
            var res = await Task.WhenAll(schemas).ConfigureAwait(false);

            executeAction(codeGenerator, res);
        }

        private static void WriteToFile(JsonSchemaCodeGenerator codeGenerator, IEnumerable<JSchema> schemas,
            string fileName, string rootNamespace = null)
        {
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                codeGenerator.WriteTo(schemas, stream, rootNamespace);
            }
        }

        private static void CompileToAssembly(JsonSchemaCodeGenerator codeGenerator, IEnumerable<JSchema> schemas, string outputAssembly, string rootNamespace = null)
        {
            using (var stream = new FileStream(outputAssembly, FileMode.Create))
            {
                codeGenerator.CompileTo(schemas, stream, rootNamespace);
            }
        }
    }
}