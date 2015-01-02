using Microsoft.Practices.Unity;

namespace TerrificNet.AssetCompiler.Compiler
{
	public class CachedAssetCompilerFactory : AssetCompilerFactory
	{
		public CachedAssetCompilerFactory(IAssetCompiler[] compilers, IUnityContainer container) : base(compilers, container)
		{
		}

		public override IAssetCompiler GetCompiler(string assetName)
		{
			return new CachedAssetCompiler(base.GetCompiler(assetName));
		}
	}
}