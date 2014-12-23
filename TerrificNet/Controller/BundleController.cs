using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TerrificNet.AssetCompiler;
using TerrificNet.AssetCompiler.Configuration;
using TerrificNet.AssetCompiler.Helpers;

namespace TerrificNet.Controller
{
	public class BundleController : ApiController
	{
		private readonly IAssetCompilerFactory _assetCompilerFactory;
		private readonly IAssetBundler _assetBundler;

		public BundleController(IAssetCompilerFactory assetCompilerFactory, IAssetBundler assetBundler)
		{
			_assetCompilerFactory = assetCompilerFactory;
			_assetBundler = assetBundler;
		}

		[HttpGet]
		public async Task<HttpResponseMessage> Get(string name)
		{
			const string config = "config.json";
			const string basePath = @"E:\Projects\finma\terrific\";

			var terrificConfig = TerrificConfig.Parse(basePath + config);
			var components = AssetHelper.GetGlobComponentsForAsset(terrificConfig.Assets[name], basePath);
			var bundle = await _assetBundler.BundleAsync(components);
			var compiler = _assetCompilerFactory.GetCompiler(name);
			var content = await compiler.CompileAsync(bundle);
			var response = new HttpResponseMessage { Content = new StringContent(content, System.Text.Encoding.Default, compiler.MimeType) };
			return response;
		}
	}
}
