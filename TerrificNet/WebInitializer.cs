using System;
using System.IO;
using Microsoft.Practices.Unity;
using TerrificNet.Configuration;
using TerrificNet.ModelProviders;
using TerrificNet.UnityModules;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.ModelProviders;
using TerrificNet.ViewEngine.ViewEngines;
using TerrificNet.ViewEngine.ViewEngines.TemplateHandler;

static public class WebInitializer
{
    public static UnityContainer Initialize(string path)
    {
        var configuration = TerrificNetHostConfigurationLoader.LoadConfiguration(Path.Combine(path, "application.json"));
        return Initialize(path, configuration);
    }

    public static UnityContainer Initialize(string path, TerrificNetHostConfiguration configuration)
    {
        var container = new UnityContainer();
        container
            .RegisterType
            <ITerrificTemplateHandlerFactory, GenericUnityTerrificTemplateHandlerFactory<DefaultTerrificTemplateHandler>>();
        container.RegisterType<INamingRule, NamingRule>();

        new DefaultUnityModule().Configure(container);

        foreach (var item in configuration.Applications.Values)
        {
            var childContainer = container.CreateChildContainer();

            var app = DefaultUnityModule.RegisterForApplication(childContainer, path, item.BasePath,
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

    public static void RegisterModelProviders(IUnityContainer childContainer)
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