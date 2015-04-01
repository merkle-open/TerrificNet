using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TerrificNet.AssetCompiler;
using TerrificNet.AssetCompiler.Helpers;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.Config;
using TerrificNet.ViewEngine.IO;

namespace TerrificNet.Controllers
{
	public class BundleController : ApiController
	{
		private readonly IAssetCompilerFactory _assetCompilerFactory;
		private readonly IAssetBundler _assetBundler;
		private readonly ITerrificNetConfig _config;
		private readonly IAssetHelper _assetHelper;
	    private readonly IFileSystem _fileSystem;

	    public BundleController(IAssetCompilerFactory assetCompilerFactory, 
            IAssetBundler assetBundler, 
            ITerrificNetConfig config, 
            IAssetHelper assetHelper,
            IFileSystem fileSystem)
		{
			_assetCompilerFactory = assetCompilerFactory;
			_assetBundler = assetBundler;
			_config = config;
			_assetHelper = assetHelper;
		    _fileSystem = fileSystem;
		}

		[HttpGet]
		public async Task<HttpResponseMessage> Get(string name)
		{
			var components = _assetHelper.GetGlobComponentsForAsset(_config.Assets[name], _fileSystem.BasePath.ToString());
			var content = await _assetBundler.BundleAsync(components).ConfigureAwait(false);
			var compiler = _assetCompilerFactory.GetCompiler(name);
			var compiledContent = await compiler.CompileAsync(content).ConfigureAwait(false);
			var response = new HttpResponseMessage { Content = new StringContent(compiledContent, System.Text.Encoding.Default, compiler.MimeType) };
			return response;
		}
	}
}
