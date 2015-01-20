using Microsoft.Practices.Unity;
using TerrificNet.AssetCompiler;
using TerrificNet.AssetCompiler.Bundler;
using TerrificNet.AssetCompiler.Compiler;
using TerrificNet.AssetCompiler.Helpers;

namespace TerrificNet.UnityModule
{
	public class TerrificBundleUnityModule : IUnityModule
	{
		public void Configure(IUnityContainer container)
		{
			container.RegisterType<IAssetCompiler, JsAssetCompiler>("Js");
			container.RegisterType<IAssetCompiler, LessAssetCompiler>("Css");
			container.RegisterType<IAssetCompilerFactory, CachedAssetCompilerFactory>();
			container.RegisterType<IAssetBundler, DefaultAssetBundler>();
			container.RegisterType<IAssetHelper, AssetHelper>();
		}
	}
}