using System.Net.Http;
using System.Web.Http;
using TerrificNet.ViewEngine.Config;

namespace TerrificNet.Controller
{
    public class TestAssetsController : StaticFileControllerBase
    {
        private readonly ITerrificNetConfig _config;

		public TestAssetsController(ITerrificNetConfig config)
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
			get { return @"E:\Projects\finma\terrific\assets\"; }
        }
    }
}