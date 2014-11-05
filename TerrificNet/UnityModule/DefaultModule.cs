using System.IO;
using System.Net.Http;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using TerrificNet.Configuration;
using TerrificNet.Generator;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.Config;
using TerrificNet.ViewEngine.ModelProviders;
using TerrificNet.ViewEngine.SchemaProviders;
using TerrificNet.ViewEngine.ViewEngines;

namespace TerrificNet.UnityModule
{
	public class DefaultUnityModule : IUnityModue
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
                container.RegisterInstance<ITerrificNetConfig>("section_" + item.Section, item); 
	        }

		    container.RegisterType<ITerrificNetConfig>(new InjectionFactory(c =>
		    {
		        var message = c.Resolve<HttpRequestMessage>();
		        var routeData = message.GetRouteData();

		        var section = (string) routeData.Values["section"];
                return c.Resolve<ITerrificNetConfig>("section_" + section);
		    }));

			container.RegisterType<IViewEngine, NustachePhysicalViewEngine>();
			container.RegisterType<IModelProvider,JsonModelProvier>();
			container.RegisterType<ISchemaProvider, SchemaMergeProvider>(new InjectionConstructor(new ResolvedParameter<NustacheViewSchemaProvider>(), new ResolvedParameter<BaseSchemaProvider>()));
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
	        foreach (var item in configuration.Applications.Values)
	        {
	            var basePath = item.BasePath;
                item.ViewPath = GetDefaultValueIfNotSet(item.DataPath, basePath, "views");
                item.AssetPath = GetDefaultValueIfNotSet(item.DataPath, basePath, "assets");
                item.DataPath = GetDefaultValueIfNotSet(item.DataPath, basePath, "project/data");
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

    public interface IUnityModue
	{
		void Configure(IUnityContainer container);
	}
}
