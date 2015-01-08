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
			var configuration = LoadConfiguration(Path.Combine(_applicationPath, "application.json"));
			foreach (var item in configuration.Applications.Values)
			{
			    var childContainer = container.CreateChildContainer();
			    var app = RegisterForApplication(childContainer, item, item.ApplicationName);
                container.RegisterInstance(item.ApplicationName, app);
			}
		}

        public static TerrificNetApplication RegisterForApplication(IUnityContainer container, ITerrificNetConfig item, string applicationName)
	    {
            var app = new TerrificNetApplication(applicationName, item, container);

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
			string content;
			using (var reader = new StreamReader(path))
			{
				content = reader.ReadToEnd();
			}

			var configuration = JsonConvert.DeserializeObject<TerrificNetHostConfiguration>(content);
			foreach (var item in configuration.Applications)
			{
				var basePath = item.Value.BasePath;
				item.Value.ApplicationName = item.Key;
				item.Value.ViewPath = GetDefaultValueIfNotSet(item.Value.DataPath, basePath, "views");
				item.Value.ModulePath = GetDefaultValueIfNotSet(item.Value.ModulePath, basePath, "components/modules");
				item.Value.AssetPath = GetDefaultValueIfNotSet(item.Value.DataPath, basePath, "assets");
				item.Value.DataPath = GetDefaultValueIfNotSet(item.Value.DataPath, basePath, "project/data");
			}

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
