using System.Web.Http.Dependencies;

namespace TerrificNet.Controllers
{
    public interface IDependencyResolverAware
    {
        IDependencyResolver DependencyResolver { get; set; }
    }
}