using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace TerrificNet.Generator.MSBuild
{
    public class CompileTask : Task
    {
        public override bool Execute()
        {
            GeneratorUtility.Execute(SourcePath, OutputAssembly, RootNamespace);
            return true;
        }

        [Required]
        public string SourcePath { get; set; }

        [Required]
        public string OutputAssembly { get; set; }

        public string RootNamespace { get; set; }
    }
}
