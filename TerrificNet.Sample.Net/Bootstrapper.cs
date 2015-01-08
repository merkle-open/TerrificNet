using System.Web.Mvc;
using Microsoft.Practices.Unity;
using TerrificNet.Configuration;
using TerrificNet.UnityModule;
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
        DefaultUnityModule.RegisterForApplication(container, new TerrificNetConfig
        {
            ApplicationName = "App",
            BasePath = @"C:\Users\mschaelle\Source\Repos\TerrificNet\TerrificNet.Sample"
        }, "App");
    }
  }
}