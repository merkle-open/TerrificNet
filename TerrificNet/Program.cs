using System;
using System.IO;
using System.Linq;
using Microsoft.Owin.Hosting;
using Microsoft.Practices.Unity;
using NuGet;
using TerrificNet.Configuration;
using TerrificNet.UnityModules;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.ViewEngines;

namespace TerrificNet
{
    class Program
    {
        private const string PathArgumentPrefix = "--path=";

        static void Main(string[] args)
        {
            const string baseAddress = "http://+:9000/";

            var path = args.FirstOrDefault(i => i.StartsWith(PathArgumentPrefix));
            if (!string.IsNullOrEmpty(path))
                path = path.Substring(PathArgumentPrefix.Length);
            else
                path = string.Empty;

            var container = new UnityContainer();
            container.RegisterType<ITerrificTemplateHandlerFactory, GenericUnityTerrificTemplateHandlerFactory<DefaultTerrificTemplateHandler>>();
            container.RegisterType<INamingRule, NamingRule>();

            new DefaultUnityModule().Configure(container);

            var configuration = TerrificNetHostConfigurationLoader.LoadConfiguration(Path.Combine(path, "application.json"));
            foreach (var item in configuration.Applications.Values)
            {
                var childContainer = container.CreateChildContainer();

                var app = DefaultUnityModule.RegisterForApplication(childContainer, Path.Combine(path, item.BasePath), item.ApplicationName, item.Section);
                container.RegisterInstance(item.ApplicationName, app);
            }

            new TerrificBundleUnityModule().Configure(container);

            // Start OWIN host
            using (WebApp.Start(baseAddress, builder => new Startup().Configuration(builder, container)))
            {
                Console.WriteLine("Started on " + baseAddress);
                Console.ReadLine();
            }

            //string installationPath = "";
            //string packageSource = "";
            //var repo = PackageRepositoryFactory.Default.CreateRepository(packageSource);
            //var packageManager = new PackageManager(repo, path);
            
        }
    }
}
