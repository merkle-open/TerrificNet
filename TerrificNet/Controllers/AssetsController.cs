using System.Net.Http;
using System.Web.Http;
using TerrificNet.Configuration;
using TerrificNet.ViewEngine.Config;
using TerrificNet.ViewEngine.IO;

namespace TerrificNet.Controllers
{
    public class AssetsController : StaticFileController
    {
        private readonly PathInfo _filePath;

        public AssetsController(ITerrificNetConfig config, IFileSystem fileSystem, ServerConfiguration serverConfiguration) 
			: base(fileSystem, serverConfiguration)
        {
            _filePath = config.AssetPath;
        }

        [HttpGet]
        public override HttpResponseMessage Get(string path)
        {
            return GetInternal(path);
        }

        protected override PathInfo FilePath
        {
            get { return _filePath; }
        }
    }
}