using Microsoft.Practices.Unity;
using TerrificNet.AssetCompiler;
using TerrificNet.AssetCompiler.Bundler;
using TerrificNet.AssetCompiler.Compiler;

namespace TerrificNet.UnityModule
{
	public class TerifficBundlerUnityModule : IUnityModue
	{
		public void Configure(IUnityContainer container)
		{
			container.RegisterType<IAssetCompiler, JsAssetCompiler>("Js");
			container.RegisterType<IAssetCompiler, LessAssetCompiler>("Css");
			container.RegisterType<IAssetCompilerFactory, AssetCompilerFactory>();
			container.RegisterType<IAssetBundler, DefaultAssetBundler>();
		}
	}
}