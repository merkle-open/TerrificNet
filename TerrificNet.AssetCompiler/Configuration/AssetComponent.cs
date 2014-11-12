using System.Collections.Generic;

namespace TerrificNet.AssetCompiler.Configuration
{
    public class AssetComponent
    {
        public IList<string> Files { get; set; }
        public IList<string> PrecompileFiles { get; set; }

        public AssetComponent()
        {
            Files = new List<string>();
            PrecompileFiles = new List<string>();
        }
    }
}
