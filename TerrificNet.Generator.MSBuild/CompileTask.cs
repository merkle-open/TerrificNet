using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using TerrificNet.Configuration;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.SchemaProviders;

namespace TerrificNet.Generator.MSBuild
{
    public class CompileTask : Task
    {
        public override bool Execute()
        {
            Execute(SourcePath, OutputAssembly);

            return true;
        }

        public static void Execute(string sourcePath, string outputAssembly)
        {
            var config = new TerrificNetConfig
            {
                BasePath = sourcePath,
                ViewPath = Path.Combine(sourcePath, "views"),
                ModulePath = Path.Combine(sourcePath, "components/modules")
            };

            var schemaProvider = new HandlebarsViewSchemaProvider();
            var repo = new TerrificTemplateRepository(config);
            var codeGenerator = new JsonSchemaCodeGenerator();

            using (var stream = new FileStream(outputAssembly, FileMode.Create))
            {
                codeGenerator.CompileTo(repo.GetAll().Select(t =>
                {
                    var schema = schemaProvider.GetSchemaFromTemplate(t);
                    schema.Title = t.Id + "Model";
                    return schema;
                }).ToList(), stream);
            }
        }

        [Required]
        public string SourcePath { get; set; }

        [Required]
        public string OutputAssembly { get; set; }
    }
}
