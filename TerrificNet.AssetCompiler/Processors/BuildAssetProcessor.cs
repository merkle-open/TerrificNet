using System.Threading.Tasks;
using TerrificNet.AssetCompiler.Configuration;

namespace TerrificNet.AssetCompiler.Processors
{
    public class BuildAssetProcessor : IAssetProcessor
    {
        #region Implementation of IAssetProcessor

        public bool Minify { get; set; }
        public Task ProcessAsync()
        {
            return ProcessAsync(TerrificConfig.Parse());
        }

        public Task ProcessAsync(string configPath)
        {
            return ProcessAsync(TerrificConfig.Parse(configPath));
        }

        public Task ProcessAsync(TerrificConfig config)
        {
            return null;
        }

        #endregion
    }
}
