using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TerrificNet.AssetCompiler;
using TerrificNet.AssetCompiler.Configuration;
using TerrificNet.AssetCompiler.Helpers;
using TerrificNet.ViewEngine.Config;

namespace TerrificNet.Controller
{
	public class BundleController : ApiController
	{
		private readonly IAssetCompilerFactory _assetCompilerFactory;
		private readonly IAssetBundler _assetBundler;
		private readonly ITerrificNetConfig _config;
		private readonly IAssetHelper _assetHelper;

		public BundleController(IAssetCompilerFactory assetCompilerFactory, IAssetBundler assetBundler, ITerrificNetConfig config, IAssetHelper assetHelper)
		{
			_assetCompilerFactory = assetCompilerFactory;
			_assetBundler = assetBundler;
			_config = config;
			_assetHelper = assetHelper;
		}

		[HttpGet]
		public async Task<HttpResponseMessage> Get(string name)
		{
			const string config = "config.json";

			var terrificConfig = TerrificConfig.Parse(_config.BasePath + config);
			var components = _assetHelper.GetGlobComponentsForAsset(terrificConfig.Assets[name], _config.BasePath);
			var content = await _assetBundler.BundleAsync(components);
			var compiler = _assetCompilerFactory.GetCompiler(name);
			var compiledContent = await compiler.CompileAsync(content);
			var response = new HttpResponseMessage { Content = new StringContent(compiledContent, System.Text.Encoding.Default, compiler.MimeType) };
			return response;
		}
	}
}
