using System.Web.Http;
using Microsoft.Practices.Unity;
using Owin;
using Unity.WebApi;

namespace TerrificNet
{
	public class Startup
	{
		// This code configures Web API. The Startup class is specified as a type
		// parameter in the WebApp.Start method.
		public void Configuration(IAppBuilder appBuilder, IUnityContainer container)
		{
			// Configure Web API for self-host. 
			var config = new HttpConfiguration();
			config.MapHttpAttributeRoutes();
			config.DependencyResolver = new UnityDependencyResolver(container);

			appBuilder.UseWebApi(config);
			
		}
	}
}