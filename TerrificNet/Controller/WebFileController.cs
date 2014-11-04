using System.Net.Http;
using System.Web.Http;

namespace TerrificNet.Controller
{
    public class WebFileController : StaticFileControllerBase
    {
        [Route("web/{*path}")]
        [HttpGet]
        public override HttpResponseMessage Get(string path)
        {
            return GetInternal(path);
        }

        protected override string FilePath
        {
            get { return "Web"; }
        }
    }
}