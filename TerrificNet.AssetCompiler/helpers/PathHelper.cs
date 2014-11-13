using System.IO;

namespace TerrificNet.AssetCompiler.Helpers
{
    static class PathHelper
    {
        public static string AssemblyDirectory
        {
            get
            {
                var assemblyLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location);
                if (assemblyLocation == null) throw new DirectoryNotFoundException("assembly directory not found");
                return assemblyLocation;
            }
        }
    }
}
