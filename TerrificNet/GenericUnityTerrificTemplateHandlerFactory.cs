using Microsoft.Practices.Unity;
using TerrificNet.ViewEngine.ViewEngines;

namespace TerrificNet
{
    public class GenericUnityTerrificTemplateHandlerFactory<T> : ITerrificTemplateHandlerFactory
        where T : ITerrificTemplateHandler
    {
        private readonly IUnityContainer _container;

        public GenericUnityTerrificTemplateHandlerFactory(IUnityContainer container)
        {
            _container = container;
        }

        public ITerrificTemplateHandler Create()
        {
            return _container.Resolve<T>();
        }
    }
}