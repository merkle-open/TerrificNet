using System;
using System.IO;
using System.Linq;
using Microsoft.Owin.Hosting;
using Microsoft.Practices.Unity;
using TerrificNet.Configuration;
using TerrificNet.ModelProviders;
using TerrificNet.UnityModules;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.ModelProviders;
using TerrificNet.ViewEngine.ViewEngines;
using TerrificNet.ViewEngine.ViewEngines.TemplateHandler;

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

            var container = Initialize(path);

            // Start OWIN host
            using (WebApp.Start(baseAddress, builder => new Startup().Configuration(builder, container)))
            {
                Console.WriteLine("Started on " + baseAddress);
                Console.ReadLine();
            }
        }

        private static UnityContainer Initialize(string path)
        {
            var container = new UnityContainer();
            container
                .RegisterType
                <ITerrificTemplateHandlerFactory, GenericUnityTerrificTemplateHandlerFactory<DefaultTerrificTemplateHandler>>();
            container.RegisterType<INamingRule, NamingRule>();

            new DefaultUnityModule().Configure(container);

            var configuration = TerrificNetHostConfigurationLoader.LoadConfiguration(Path.Combine(path, "application.json"));
            foreach (var item in configuration.Applications.Values)
            {
                var childContainer = container.CreateChildContainer();

                var app = DefaultUnityModule.RegisterForApplication(childContainer, Path.Combine(path, item.BasePath),
                    item.ApplicationName, item.Section);
                container.RegisterInstance(item.ApplicationName, app);
            }

            foreach (var app in container.ResolveAll<TerrificNetApplication>())
            {
                RegisterModelProviders(app.Container);

                foreach (var template in app.Container.Resolve<ITemplateRepository>().GetAll())
                {
                    Console.WriteLine(template.Id);
                }
            }

            new TerrificBundleUnityModule().Configure(container);
            return container;
        }

        private static void RegisterModelProviders(IUnityContainer childContainer)
        {
            var modelProvider = (DefaultModelProvider) childContainer.Resolve<IModelProvider>();
            var repo = childContainer.Resolve<ITemplateRepository>();

            TemplateInfo templateInfo;
            if (repo.TryGetTemplate("components/modules/ApplicationOverview/ApplicationOverview", null,
                out templateInfo))
                modelProvider.RegisterProviderForTemplate(templateInfo,
                    childContainer.Resolve<ApplicationOverviewModelProvider>());
        }
    }
}
