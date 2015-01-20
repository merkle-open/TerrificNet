using System.IO;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
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

			var configuration = LoadConfiguration(Path.Combine(_applicationPath, "application.json"));
			foreach (var item in configuration.Applications.Values)
			{
			    var childContainer = container.CreateChildContainer();

			    var app = RegisterForApplication(childContainer, item.BasePath, item.ApplicationName, item.Section);
			    container.RegisterInstance(item.ApplicationName, app);
			}
		}

	    public static TerrificNetApplication RegisterForApplication(IUnityContainer childContainer, string basePath, string applicationName, string section)
	    {
	        var config = LoadTerrificConfiguration(basePath);

	        var app = RegisterForApplication(childContainer, config, applicationName, section);
	        return app;
	    }

	    private static ITerrificNetConfig LoadTerrificConfiguration(string basePath)
	    {
	        var configPath = Path.Combine(basePath, "config.json");
	        var config = ReadJson<TerrificNetConfig>(configPath);

	        config.BasePath = basePath;
            config.ViewPath = Path.Combine(basePath, GetDefaultValueIfNotSet(config.ViewPath, basePath, "views"));
            config.ModulePath = Path.Combine(basePath, GetDefaultValueIfNotSet(config.ModulePath, basePath, "components/modules"));
            config.AssetPath = Path.Combine(basePath, GetDefaultValueIfNotSet(config.AssetPath, basePath, "assets"));
            config.DataPath = Path.Combine(basePath, GetDefaultValueIfNotSet(config.DataPath, basePath, "project/data"));

	        return config;
	    }

	    private static TerrificNetApplication RegisterForApplication(IUnityContainer container, ITerrificNetConfig item, string applicationName, string section)
	    {
            var app = new TerrificNetApplication(applicationName, section, item, container);

	        container.RegisterInstance(app);
	        container.RegisterInstance(item);

	        RegisterApplicationSpecific(container);

	        return app;
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

		private static TerrificNetHostConfiguration LoadConfiguration(string path)
		{
            var configuration = ReadJson<TerrificNetHostConfiguration>(path);
		    foreach (var item in configuration.Applications)
			{
				var basePath = item.Value.BasePath;
				item.Value.ApplicationName = item.Key;
			}

			return configuration;
		}

	    private static T ReadJson<T>(string path)
	    {
	        string content;
	        using (var reader = new StreamReader(path))
	        {
	            content = reader.ReadToEnd();
	        }

	        var configuration = JsonConvert.DeserializeObject<T>(content);
	        return configuration;
	    }

	    private static string GetDefaultValueIfNotSet(string value, string basePath, string defaultLocation)
		{
			if (string.IsNullOrEmpty(value))
				return Path.Combine(basePath, defaultLocation);

			return value;
		}
	}
}
