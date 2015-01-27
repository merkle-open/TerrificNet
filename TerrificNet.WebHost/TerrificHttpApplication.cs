using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using TerrificNet.Configuration;

namespace TerrificNet.WebHost
{
    public class TerrificHttpApplication : HttpApplication
    {
        public override void Init()
        {
            base.Init();

            GlobalConfiguration.Configure(config =>
            {
                var configuration = new TerrificNetHostConfiguration
                {
                    Applications = new Dictionary<string, TerrificNetApplicationConfiguration>
                    {
                        { "administration", new TerrificNetApplicationConfiguration
                        {
                            BasePath = "zip://Web.zip/Web",
                            ApplicationName = "administration",
                            Section = "web/"
                        } 
                        },
                        { "application", new TerrificNetApplicationConfiguration
                        {
                            BasePath = "",
                            ApplicationName = "application",
                            Section = ""
                        }}
                    }
                };

                var container = WebInitializer.Initialize(this.Server.MapPath("/"), configuration);
                new Startup().Configuration(container, config);
            });
        }
    }
}
