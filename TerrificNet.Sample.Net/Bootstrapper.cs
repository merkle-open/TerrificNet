using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using TerrificNet.Configuration;
using TerrificNet.Mvc;
using TerrificNet.UnityModule;
using TerrificNet.ViewEngine;
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

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();    
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
            var basePath = Path.Combine(rootPath, @"..\TerrificNet.Sample");
            DefaultUnityModule.RegisterForApplication(container, new TerrificNetConfig
            {
                ApplicationName = "App",
                BasePath = basePath,
                ViewPath = Path.Combine(basePath, @"views"),
                ModulePath = Path.Combine(basePath, @"components\modules"),
            }, "App");
        }
    }
}