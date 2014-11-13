using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TerrificNet.AssetCompiler.Configuration
{
    public class AssetComponents
    {
        public IList<Func<Task<string>>> Files { get; private set; }
        public IList<Func<Task<string>>> Dependencies { get; private set; }

        public AssetComponents()
        {
            Files = new List<Func<Task<string>>>();
            Dependencies = new List<Func<Task<string>>>();
        }
    }
}
