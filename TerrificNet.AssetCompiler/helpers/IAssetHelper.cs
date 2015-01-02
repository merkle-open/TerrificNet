using System.Collections.Generic;
using System.Threading.Tasks;
using TerrificNet.AssetCompiler.Configuration;

namespace TerrificNet.AssetCompiler.Helpers
{
	public interface IAssetHelper
	{
		AssetComponents GetGlobComponentsForAsset(IEnumerable<string> assetPaths, string basePath);
	}
}