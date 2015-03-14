using Newtonsoft.Json.Linq;
using TerrificNet.ViewEngine.Globalization;
using Veil;

namespace TerrificNet.ViewEngine.TemplateHandler
{
    public class DefaultTerrificTemplateHandler : ITerrificTemplateHandler
    {
        private readonly IViewEngine _viewEngine;
        private readonly IModelProvider _modelProvider;
        private readonly ITemplateRepository _templateRepository;
	    private readonly ILabelService _labelService;
        private readonly IModuleRepository _moduleRepository;

        public DefaultTerrificTemplateHandler(IViewEngine viewEngine, IModelProvider modelProvider,
            ITemplateRepository templateRepository, ILabelService labelService, IModuleRepository moduleRepository)
        {
            _viewEngine = viewEngine;
            _modelProvider = modelProvider;
            _templateRepository = templateRepository;
		    _labelService = labelService;
            _moduleRepository = moduleRepository;
        }

        public void RenderPlaceholder(object model, string key, RenderingContext context)
        {
            ViewDefinition definition;
            var tmp = model as JObject;
            if (tmp != null)
            {
                definition = ViewDefinition.FromJObject<ViewDefinition>(tmp);
            }
            else
            {
                definition = model as ViewDefinition;
            }

            if (definition == null || definition.Placeholder == null)
                return;

            var placeholder = definition.Placeholder;

            ViewDefinition[] definitions;

            if (!placeholder.TryGetValue(key, out definitions))
                return;

            foreach (var placeholderConfig in definitions)
            {
                placeholderConfig.Render(this, model, new RenderingContext(context.Writer, context));
            }
        }

        public void RenderModule(string moduleId, string skin, RenderingContext context)
        {
	        string dataVariation = null;
	        object dataVariationObj;
	        if (context.Data.TryGetValue("data_variation", out dataVariationObj))
				dataVariation = dataVariationObj as string;

            // TODO: Use async
            var moduleDefinition = _moduleRepository.GetModuleDefinitionByIdAsync(moduleId).Result;
            if (moduleDefinition != null)
            {
                TemplateInfo templateInfo;
                if (string.IsNullOrEmpty(skin) || moduleDefinition.Skins == null || !moduleDefinition.Skins.TryGetValue(skin, out templateInfo))
                    templateInfo = moduleDefinition.DefaultTemplate;

                IView view;
                if (_viewEngine.TryCreateView(templateInfo, out view))
                {
                    // TODO: make async
                    var moduleModel = _modelProvider.GetModelForModuleAsync(moduleDefinition, dataVariation).Result;
                    view.Render(moduleModel, new RenderingContext(context.Writer, context));
                    return;
                }
            }

            context.Writer.Write("Problem loading template " + moduleId + (!string.IsNullOrEmpty(skin) ? "-" + skin : string.Empty));
        }

		public void RenderLabel(string key, RenderingContext context)
	    {
		    context.Writer.Write(_labelService.Get(key));
	    }

        public void RenderPartial(string template, object model, RenderingContext context)
        {
            // TODO: Use async
            var templateInfo = _templateRepository.GetTemplateAsync(template).Result;
            if (templateInfo != null)
            {
                IView view;
                if (_viewEngine.TryCreateView(templateInfo, out view))
                {
                    view.Render(model, new RenderingContext(context.Writer, context));
                    return;
                }
            }
            context.Writer.Write("Problem loading template " + template);
        }
    }
}