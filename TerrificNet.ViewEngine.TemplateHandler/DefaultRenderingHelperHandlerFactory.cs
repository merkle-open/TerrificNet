using System.Collections.Generic;
using TerrificNet.ViewEngine.Client;
using TerrificNet.ViewEngine.TemplateHandler.Grid;
using TerrificNet.ViewEngine.ViewEngines;
using Veil.Helper;

namespace TerrificNet.ViewEngine.TemplateHandler
{
    public class DefaultRenderingHelperHandlerFactory : IHelperHandlerFactory, IRenderingHelperHandlerFactory
    {
        private readonly ITerrificTemplateHandlerFactory _terrificTemplateHandlerFactory;
        private readonly ITemplateRepository _templateRepository;
        private readonly ISchemaProviderFactory _schemaProviderFactory;
		private readonly IClientTemplateGeneratorFactory _clientTemplateGeneratorFactory;

	    public DefaultRenderingHelperHandlerFactory(
            ITerrificTemplateHandlerFactory terrificTemplateHandlerFactory,
            ITemplateRepository templateRepository,
            ISchemaProviderFactory schemaProviderFactory,
			IClientTemplateGeneratorFactory clientTemplateGeneratorFactory)
        {
            _terrificTemplateHandlerFactory = terrificTemplateHandlerFactory;
            _templateRepository = templateRepository;
            _schemaProviderFactory = schemaProviderFactory;
	        _clientTemplateGeneratorFactory = clientTemplateGeneratorFactory;
        }

        public IEnumerable<IRenderingHelperHandler> Create()
        {
            yield return new TerrificRenderingHelperHandler(_terrificTemplateHandlerFactory.Create(), _schemaProviderFactory.Create(), _templateRepository, _clientTemplateGeneratorFactory.Create());
            yield return new GridHelperHandler();
            yield return new GridWidthHelperHandler();
			yield return new GridComponentWidthHelperHandler();
            yield return new TemplateIdHelperHandler();
        }

        IEnumerable<IHelperHandler> IHelperHandlerFactory.Create()
        {
            return Create();
        }
    }
}