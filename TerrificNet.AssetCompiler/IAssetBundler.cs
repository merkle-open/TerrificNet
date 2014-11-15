using System.Threading.Tasks;
using TerrificNet.AssetCompiler.Configuration;

namespace TerrificNet.AssetCompiler
{
    public interface IAssetBundler
    {
        Task<string> BundleAsync(AssetComponents components);
    }
}
