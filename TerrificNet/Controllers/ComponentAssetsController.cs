using System.Net.Http;
using System.Web.Http;
using TerrificNet.ViewEngine.Config;

namespace TerrificNet.Controllers
{
	public class ComponentAssetsController : StaticFileControllerBase
	{
		private readonly ITerrificNetConfig _config;

		public ComponentAssetsController(ITerrificNetConfig config)
		{
			_config = config;
		}

		[HttpGet]
		public override HttpResponseMessage Get(string path)
		{
			return GetInternal(path);
		}

		protected override string FilePath
		{
			get { return _config.ModulePath; }
		}
	}
}