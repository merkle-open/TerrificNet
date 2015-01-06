using System.IO;
using System.Net.Http;
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
using Veil.Helper;

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
				var app = new TerrificNetApplication(item.ApplicationName, item, childContainer);
				childContainer.RegisterInstance(app);
				childContainer.RegisterInstance<ITerrificNetConfig>(item);

				RegisterApplicationSpecific(childContainer);

				container.RegisterInstance(item.ApplicationName, app);
			}
		}

		private static void RegisterApplicationSpecific(IUnityContainer container)
		{
			//container.RegisterType<IViewEngine, NustachePhysicalViewEngine>();
			container.RegisterType<IViewEngine, VeilPhysicalViewEngine>();
			container.RegisterType<ICacheProvider, MemoryCacheProvider>();
			container.RegisterType<IModelProvider, JsonModelProvider>();
			container.RegisterType<ISchemaProvider, SchemaMergeProvider>(
				new InjectionConstructor(new ResolvedParameter<NustacheViewSchemaProvider>(),
					new ResolvedParameter<BaseSchemaProvider>()));
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
