using System.IO;
using Microsoft.Practices.Unity;
using TerrificNet.Configuration;
using TerrificNet.Generator;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.Cache;
using TerrificNet.ViewEngine.Config;
using TerrificNet.ViewEngine.ModelProviders;
using TerrificNet.ViewEngine.SchemaProviders;
using TerrificNet.ViewEngine.ViewEngines;

namespace TerrificNet.UnityModule
{
	public class DefaultUnityModule : IUnityModule
	{
		private readonly string _applicationPath;

		public DefaultUnityModule(string applicationPath)
		{
			_applicationPath = applicationPath ?? string.Empty;
		}

		public void Configure(IUnityContainer container)
		{
            container.RegisterType<ITemplateRepository, TerrificTemplateRepository>();

			var configuration = TerrificNetHostConfigurationLoader.LoadConfiguration(Path.Combine(_applicationPath, "application.json"));
			foreach (var item in configuration.Applications.Values)
			{
			    var childContainer = container.CreateChildContainer();

			    var app = RegisterForApplication(childContainer, item.BasePath, item.ApplicationName, item.Section);
			    container.RegisterInstance(item.ApplicationName, app);
			}
		}

	    private static TerrificNetApplication RegisterForApplication(IUnityContainer childContainer, string basePath, string applicationName, string section)
	    {
	        var config = ConfigurationLoader.LoadTerrificConfiguration(basePath);

	        var app = RegisterForApplication(childContainer, config, applicationName, section);
	        return app;
	    }

	    private static TerrificNetApplication RegisterForApplication(IUnityContainer container, ITerrificNetConfig item, string applicationName, string section)
	    {
            var app = new TerrificNetApplication(applicationName, section, item, container);

	        container.RegisterInstance(app);
	        RegisterForConfiguration(container, item);

	        return app;
	    }

	    public static void RegisterForConfiguration(IUnityContainer container, ITerrificNetConfig item)
	    {
	        container.RegisterInstance(item);

	        RegisterApplicationSpecific(container);
	    }

	    private static void RegisterApplicationSpecific(IUnityContainer container)
		{
			container.RegisterType<IViewEngine, VeilViewEngine>();
			container.RegisterType<ICacheProvider, MemoryCacheProvider>();
			container.RegisterType<IModelProvider, JsonModelProvider>();
			container.RegisterType<ISchemaProvider, SchemaMergeProvider>(
				new InjectionConstructor(new ResolvedParameter<HandlebarsViewSchemaProvider>(),
					new ResolvedParameter<PhysicalSchemaProvider>()));
			container.RegisterType<IJsonSchemaCodeGenerator, JsonSchemaCodeGenerator>();
		}
	}
}
