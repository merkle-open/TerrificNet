using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using TerrificNet.Configuration;
using TerrificNet.ViewEngine;

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

	            var serverConfiguration = ServerConfiguration.LoadConfiguration(this.Server.MapPath("/server.json"));
                var container = WebInitializer.Initialize(this.Server.MapPath("/"), configuration, serverConfiguration);
				
				container.RegisterType<IModelTypeProvider, DummyModelTypeProvider>();

                new Startup().Configuration(container, config);
            });
            RouteTable.Routes.RouteExistingFiles = true;
        }

		private class DummyModelTypeProvider : IModelTypeProvider
		{
			public Task<Type> GetModelTypeFromTemplateAsync(TemplateInfo templateInfo)
			{
				return Task.FromResult<Type>(typeof(object));
			}
		}
    }
}
