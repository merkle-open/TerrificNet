
using System.Threading.Tasks;
using TerrificNet.AssetCompiler.Configuration;

namespace TerrificNet.AssetCompiler
{
    public interface IAssetProcessor
    {
        bool Minify { get; set; }
        Task ProcessAsync();
        Task ProcessAsync(string configPath);
        Task ProcessAsync(TerrificConfig config);
    }
}
