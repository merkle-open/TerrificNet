using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using TerrificNet.Configuration;

namespace TerrificNet.WebHost
{
    public class TerrificHttpApplication : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs args)
        {
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
