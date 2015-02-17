using System.Collections.Generic;
using TerrificNet.ViewEngine.ViewEngines.TemplateHandler;
using TerrificNet.ViewEngine.ViewEngines.TemplateHandler.Grid;
using Veil.Helper;

namespace TerrificNet.ViewEngine.ViewEngines
{
    public class DefaultTerrificHelperHandlerFactory : IHelperHandlerFactory, ITerrificHelperHandlerFactory
    {
        private readonly ITerrificTemplateHandlerFactory _terrificTemplateHandlerFactory;
        private readonly ITemplateRepository _templateRepository;
        private readonly ISchemaProviderFactory _schemaProviderFactory;

        public DefaultTerrificHelperHandlerFactory(
            ITerrificTemplateHandlerFactory terrificTemplateHandlerFactory,
            ITemplateRepository templateRepository,
            ISchemaProviderFactory schemaProviderFactory)
        {
            _terrificTemplateHandlerFactory = terrificTemplateHandlerFactory;
            _templateRepository = templateRepository;
            _schemaProviderFactory = schemaProviderFactory;
        }

        public IEnumerable<ITerrificHelperHandler> Create()
        {
            yield return new TerrificHelperHandler(_terrificTemplateHandlerFactory.Create(), _schemaProviderFactory.Create(), _templateRepository);
            yield return new GridHelperHandler();
            yield return new GridWidthHelperHandler();
			yield return new GridComponentWidthHelperHandler();
        }

        IEnumerable<IHelperHandler> IHelperHandlerFactory.Create()
        {
            return Create();
        }
    }
}