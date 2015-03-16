using System.Web.Http;
using System.Web.Http.Dispatcher;
using Microsoft.Practices.Unity;
using TerrificNet.Dispatcher;
using TerrificNet.UnityModules;
using Unity.WebApi;

namespace TerrificNet
{
	public class Startup
	{
		public void Configuration(IUnityContainer container, HttpConfiguration config)
		{
			config.Routes.MapHttpRoute(
				name: "AdministrationHome",
				routeTemplate: "web/",
				defaults: new { controller = "Home", action = "Index", section = "web/" }
				);

			config.Routes.MapHttpRoute(
				name: "AdministrationModuleDetail",
				routeTemplate: "web/module/{action}",
				defaults: new { controller = "ModuleDetail", action = "Index", section = "web/" }
				);

			config.Routes.MapHttpRoute(
				name: "AdministrationDataEdit",
				routeTemplate: "web/edit",
				defaults: new { controller = "DataEdit", action = "Index", section = "web/" }
				);

			config.Routes.MapHttpRoute(
				name: "AdministrationDataEditAdvanced",
				routeTemplate: "web/edit_advanced",
				defaults: new { controller = "DataEdit", action = "IndexAdvanced", section = "web/" }
			);

            config.Routes.MapHttpRoute(
                name: "PageEditor",
                routeTemplate: "web/page_edit",
                defaults: new { controller = "PageEdit", action = "Index", section = "web/" }
            );

            config.Routes.MapHttpRoute(
                name: "CoreFiles",
                routeTemplate: "$tcn/{*path}",
                defaults: new { controller = "staticfile" }
            );

			foreach (var application in container.ResolveAll<TerrificNetApplication>())
			{
				var section = application.Section;

				MapArea(config, application.Container, section);
			}

			config.DependencyResolver = new UnityDependencyResolver(container);
			config.Services.Replace(typeof(IHttpControllerActivator), new ApplicationSpecificControllerActivator(config));

		}

		private static void MapArea(HttpConfiguration config, IUnityContainer container, string section = null)
		{
			config.Routes.MapHttpRoute(
				name: "ModelRoot" + section,
				routeTemplate: section + "model/{*path}",
				defaults: new { controller = "model", section = section }
				);
			config.Routes.MapHttpRoute(
				name: "ModuleSchemaRoot" + section,
				routeTemplate: section + "module_schema/{*path}",
				defaults: new { controller = "moduleschema", section = section }
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
				name: "ClientRoot" + section,
				routeTemplate: section + "js/{*path}",
				defaults: new { controller = "clienttemplate", section = section }
				);
			config.Routes.MapHttpRoute(
				name: "AssetsRoot" + section,
				routeTemplate: section + "assets/{*path}",
				defaults: new { controller = "assets", section = section }
				);
			config.Routes.MapHttpRoute(
				name: "BundleRoot" + section,
				routeTemplate: section + "bundle_{name}",
				defaults: new { controller = "bundle", section = section }
				);
			config.Routes.MapHttpRoute(
				name: section + "TemplateRoot",
				routeTemplate: section + "{*path}",
				defaults: new { controller = "template", section = section },
				constraints: new { path = container.Resolve<ValidTemplateRouteConstraint>() }
				);
			config.Routes.MapHttpRoute(
				name: section + "TemplateRootDefault",
				routeTemplate: section,
				defaults: new { controller = "template", path = "index", section = section },
				constraints: new { path = container.Resolve<ValidTemplateRouteConstraint>() }
				);
			config.Routes.MapHttpRoute(
				name: section + "StaticFile",
				routeTemplate: section + "{*path}",
				defaults: new { controller = "staticfile", section = section }
				);
		}
	}
}