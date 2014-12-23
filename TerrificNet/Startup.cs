using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;
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

			//config.MapHttpAttributeRoutes();

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


            MapArea(config, "web/");
            MapArea(config);

		    config.DependencyResolver = new UnityDependencyResolver(container);
		    config.MessageHandlers.Add(new InjectHttpRequestMessageToContainerHandler());

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
				name: "TestAssetsRoot" + section,
				routeTemplate: section + "bundle/assets/{*path}",
				defaults: new { controller = "testassets", section = section }
				);
			config.Routes.MapHttpRoute(
				name: "BundleRoot" + section,
				routeTemplate: section + "bundle/{name}",
				defaults: new { controller = "bundle", section = section }
				);
	        config.Routes.MapHttpRoute(
                name: section + "TemplateRoot" + section,
	            routeTemplate: section + "{*path}",
                defaults: new { controller = "template", section = section }
	            );
	    }
	}

    public class InjectHttpRequestMessageToContainerHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var container = request.GetDependencyScope().GetService(typeof(IUnityContainer)) as IUnityContainer;
            if (container != null)
                container.RegisterInstance(request);

            return base.SendAsync(request, cancellationToken);
        }
    }
}