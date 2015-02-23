using System.IO;
using System.Web.Hosting;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using TerrificNet.Mvc;
using TerrificNet.UnityModules;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.Config;
using TerrificNet.ViewEngine.IO;
using TerrificNet.ViewEngine.TemplateHandler;
using TerrificNet.ViewEngine.ViewEngines;
using Unity.Mvc4;

namespace TerrificNet.Sample.Net
{
    public static class Bootstrapper
    {
        public static IUnityContainer Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            return container;
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        }

        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<ITemplateRepository, TerrificTemplateRepository>();
            container
                .RegisterType<ITerrificTemplateHandlerFactory, GenericUnityTerrificTemplateHandlerFactory<MvcTerrificTemplateHandler>>();

            container.RegisterType<INamingRule, NamingRule>();
            container.RegisterType<IModelTypeProvider>(new InjectionFactory(u => new StaticModelTypeProvider("TerrificNet.Sample.Net.Models", "TerrificNet.Sample.Net.Models", u.Resolve<INamingRule>(), u.Resolve<ISchemaProvider>())));

            var rootPath = HostingEnvironment.MapPath("~/");
			var fileSystem = new FileSystem(Path.Combine(rootPath, @"..\TerrificNet.Sample"));

	        container.RegisterInstance<IFileSystem>(fileSystem);

	        var config = ConfigurationLoader.LoadTerrificConfiguration(string.Empty, fileSystem);
            DefaultUnityModule.RegisterForConfiguration(container, config);
        }
    }
}