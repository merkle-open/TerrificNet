using System.Collections.Generic;

namespace TerrificNet.AssetCompiler.Configuration
{
    public class AssetComponents
    {
        public IList<string> Files { get; set; }
        public IList<string> Dependencies { get; set; }

        public AssetComponents()
        {
            Files = new List<string>();
            Dependencies = new List<string>();
        }
    }
}
