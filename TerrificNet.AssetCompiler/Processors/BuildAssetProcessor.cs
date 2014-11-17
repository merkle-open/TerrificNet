using Microsoft.Practices.Unity;
using System.Collections.Generic;
using System.Threading.Tasks;
using TerrificNet.AssetCompiler.Helpers;

namespace TerrificNet.AssetCompiler.Processors
{
    public class BuildAssetProcessor : IAssetProcessor
    {
        private readonly IUnityContainer _container;

        public BuildAssetProcessor(IUnityContainer container)
        {
            _container = container;
        }

        #region Implementation of IAssetProcessor

        public async Task<string> ProcessAsync(KeyValuePair<string, string[]> assetConfig, ProcessorFlags flags)
        {
            var bundler = _container.Resolve<IAssetBundler>();
            var result = await bundler.BundleAsync(AssetHelper.GetGlobComponentsForAsset(assetConfig.Value));

            if (flags.HasFlag(ProcessorFlags.Minify))
            {
                var factory = _container.Resolve<IAssetCompilerFactory>();
                var compiler = factory.GetCompiler(assetConfig.Key);

                result = await compiler.CompileAsync(result);
            }

            return result;
        }

        #endregion
    }
}
