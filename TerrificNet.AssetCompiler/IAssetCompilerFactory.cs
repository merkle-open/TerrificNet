
namespace TerrificNet.AssetCompiler
{
    public interface IAssetCompilerFactory
    {
        IAssetCompiler GetCompiler(string assetName);
    }
}
