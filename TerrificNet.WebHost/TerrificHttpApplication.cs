using System.Web;
using System.Web.Http;

namespace TerrificNet.WebHost
{
    public class TerrificHttpApplication : HttpApplication
    {
        public override void Init()
        {
            base.Init();

            GlobalConfiguration.Configure(config =>
            {
                var container = WebInitializer.Initialize(string.Empty);
                new Startup().Configuration(container, config);
            });
        }
    }
}
