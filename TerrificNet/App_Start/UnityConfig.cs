using Microsoft.Practices.Unity;
using System.Web.Http;
using TerrificNet.Controller;
using Unity.WebApi;

namespace TerrificNet
{
    public static class UnityConfig
    {
        public static void RegisterComponents(HttpConfiguration config)
        {
			var container = new UnityContainer();            
            config.DependencyResolver = new UnityDependencyResolver(container);

            container.RegisterType<IViewEngine, PhysicalViewEngine>(new InjectionConstructor(@"C:\Users\mschaelle\Source\Repos\TerrificNet\TerrificNet.Sample\views"));
            container.RegisterType<IModelProvider, JsonModelProvier>(new InjectionConstructor(@"C:\Users\mschaelle\Source\Repos\TerrificNet\TerrificNet.Sample\project\data"));
        }
    }
}