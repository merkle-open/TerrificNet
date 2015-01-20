using System.Web.Http.Dependencies;

namespace TerrificNet.Dispatcher
{
    public interface IDependencyResolverAware
    {
        IDependencyResolver DependencyResolver { get; set; }
    }
}