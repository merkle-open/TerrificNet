using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;
using Microsoft.Practices.Unity;
using Owin;
using TerrificNet.Dispatcher;
using TerrificNet.UnityModules;
using Unity.WebApi;

namespace TerrificNet
{
	public class Startup
	{
		public void Configuration(IAppBuilder appBuilder, IUnityContainer container)
		{
			var config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
                name: "ViewIndex",
                routeTemplate: "web/index.html",
                defaults: new { controller = "viewIndex", section="web/" }
                );

            config.Routes.MapHttpRoute(
                name: "ViewIndexDefault",
                routeTemplate: "",
                defaults: new { controller = "viewIndex", section = "web/" }
            );

		    foreach (var application in container.ResolveAll<TerrificNetApplication>())
		    {
                MapArea(config, application.Section);
		    }

            config.DependencyResolver = new UnityDependencyResolver(container);
            config.Services.Replace(typeof(IHttpControllerActivator), new ApplicationSpecificControllerActivator(config));
            //config.Services.Replace(typeof(IHttpControllerSelector), new ApplicationSpecificControllerSelector(config));

			appBuilder.UseWebApi(config);
		}

	    private static void MapArea(HttpConfiguration config, string section = null)
	    {
	        IHttpRoute route;
	        config.Routes.MapHttpRoute(
	            name: "ModelRoot" + section,
	            routeTemplate: section + "model/{*path}",
	            defaults: new {controller = "model", section = section }
	            );
	        config.Routes.MapHttpRoute(
	            name: "SchemaRoot" + section,
                routeTemplate: section + "schema/{*path}",
                defaults: new { controller = "schema", section = section }
	            );
			config.Routes.MapHttpRoute(
				name: "GenerateRoot" + section,
				routeTemplate: section + "generate/{*path}",
				defaults: new { controller = "generate", section = section }
				);
			config.Routes.MapHttpRoute(
				name: "AssetsRoot" + section,
				routeTemplate: section + "assets/{*path}",
				defaults: new { controller = "assets", section = section }
				);
			config.Routes.MapHttpRoute(
				name: "ComponentAssetsRoot" + section,
				routeTemplate: section + "components/modules/{*path}",
				defaults: new { controller = "ComponentAssets", section = section }
				);
			config.Routes.MapHttpRoute(
				name: "BundleRoot" + section,
				routeTemplate: section + "bundle_{name}",
				defaults: new { controller = "bundle", section = section }
				);
	        config.Routes.MapHttpRoute(
                name: section + "TemplateRoot" + section,
	            routeTemplate: section + "{*path}",
                defaults: new { controller = "template", section = section }
	            );
	    }
	}
}