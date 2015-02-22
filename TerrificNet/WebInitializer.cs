using System;
using System.IO;
using Microsoft.Practices.Unity;
using TerrificNet.Configuration;
using TerrificNet.UnityModules;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.TemplateHandler;
using TerrificNet.ViewEngine.ViewEngines;

namespace TerrificNet
{
	static public class WebInitializer
	{
		public static UnityContainer Initialize(string path)
		{
			var configuration = TerrificNetHostConfigurationLoader.LoadConfiguration(Path.Combine(path, "application.json"));
			var serverConfiguration = ServerConfiguration.LoadConfiguration(Path.Combine(path, "server.json"));

			return Initialize(path, configuration, serverConfiguration);
		}

		public static UnityContainer Initialize(string path, TerrificNetHostConfiguration configuration, ServerConfiguration serverConfiguration)
		{
			var container = new UnityContainer();
			container
				.RegisterType
				<ITerrificTemplateHandlerFactory, GenericUnityTerrificTemplateHandlerFactory<DefaultTerrificTemplateHandler>>();
			container.RegisterType<INamingRule, NamingRule>();
			container.RegisterInstance(serverConfiguration);

			new DefaultUnityModule().Configure(container);

		    container.RegisterInstance<IFileSystem>(new FileSystem(path));

			foreach (var item in configuration.Applications.Values)
			{
				var childContainer = container.CreateChildContainer();

				var app = DefaultUnityModule.RegisterForApplication(childContainer, path, item.BasePath,
					item.ApplicationName, item.Section);
				container.RegisterInstance(item.ApplicationName, app);
			}

			foreach (var app in container.ResolveAll<TerrificNetApplication>())
			{
				foreach (var template in app.Container.Resolve<ITemplateRepository>().GetAll())
				{
					Console.WriteLine(template.Id);
				}
			}

			new TerrificBundleUnityModule().Configure(container);
			return container;
		}
	}
}