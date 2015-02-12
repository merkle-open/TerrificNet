using Newtonsoft.Json.Linq;
using TerrificNet.ViewEngine.Globalization;

namespace TerrificNet.ViewEngine.ViewEngines.TemplateHandler
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
                placeholderConfig.Render(this, model, context);
            }
        }

        public void RenderModule(string moduleId, string skin, RenderingContext context)
        {
	        string dataVariation = null;
	        object dataVariationObj;
	        if (context.Data.TryGetValue("data_variation", out dataVariationObj))
				dataVariation = dataVariationObj as string;

            ModuleDefinition moduleDefinition;
            if (_moduleRepository.TryGetModuleDefinitionById(moduleId, out moduleDefinition))
            {
                TemplateInfo templateInfo;
                if (string.IsNullOrEmpty(skin) || moduleDefinition.Skins == null || !moduleDefinition.Skins.TryGetValue(skin, out templateInfo))
                    templateInfo = moduleDefinition.DefaultTemplate;

                IView view;
                if (_viewEngine.TryCreateView(templateInfo, out view))
                {
                    var moduleModel = _modelProvider.GetModelForModule(moduleDefinition, dataVariation);
                    view.Render(moduleModel, context);
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
            TemplateInfo templateInfo;
            if (_templateRepository.TryGetTemplate(template, out templateInfo))
            {
                IView view;
                if (_viewEngine.TryCreateView(templateInfo, out view))
                {
                    view.Render(model, context);
                    return;
                }
            }
            context.Writer.Write("Problem loading template " + template);
        }
    }
}