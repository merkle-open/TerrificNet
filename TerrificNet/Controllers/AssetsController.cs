using System.Net.Http;
using System.Web.Http;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.Config;

namespace TerrificNet.Controllers
{
    public class AssetsController : StaticFileControllerBase
    {
        private readonly ITerrificNetConfig _config;

        public AssetsController(ITerrificNetConfig config, IFileSystem fileSystem) : base(fileSystem)
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
            get { return _config.AssetPath; }
        }
    }
}