using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using TerrificNet.AssetCompiler.Helpers;

namespace TerrificNet.AssetCompiler.Processors
{
	public class BuildAssetProcessor : IAssetProcessor
	{
		private readonly IUnityContainer _container;
		private readonly IAssetHelper _assetHelper;

		public BuildAssetProcessor(IUnityContainer container, IAssetHelper assetHelper)
		{
			_container = container;
			_assetHelper = assetHelper;
		}

		public async Task<string> ProcessAsync(string name, string[] files, ProcessorFlags flags, string basePath)
		{
			var bundler = _container.Resolve<IAssetBundler>();
			var result = await bundler.BundleAsync(_assetHelper.GetGlobComponentsForAsset(files, basePath));

			if (flags.HasFlag(ProcessorFlags.Minify))
			{
				var factory = _container.Resolve<IAssetCompilerFactory>();
				var compiler = factory.GetCompiler(name);

				result = await compiler.CompileAsync(result);
			}

			return result;
		}
	}
}
