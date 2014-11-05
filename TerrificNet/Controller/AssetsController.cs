using System.Net.Http;
using System.Web.Http;
using TerrificNet.ViewEngine.Config;

namespace TerrificNet.Controller
{
    public class AssetsController : StaticFileControllerBase
    {
        private readonly ITerrificNetConfig _config;

        public AssetsController(ITerrificNetConfig config)
        {
            _config = config;
        }

        [Route("assets/{*path}")]
        [HttpGet]
        public override HttpResponseMessage Get(string path)
        {
            return GetInternal(path);
        }

        protected override string FilePath
        {
            get { return _config.AssetPath; }
        }
    }
}