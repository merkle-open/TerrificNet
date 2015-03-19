using System.Linq;
using System.Threading.Tasks;
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

        public Task RenderPlaceholderAsync(object model, string key, RenderingContext context)
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
                return Task.FromResult(false);

            var placeholder = definition.Placeholder;

            ViewDefinition[] definitions;

            if (!placeholder.TryGetValue(key, out definitions))
                return Task.FromResult(false);

            return
                Task.WhenAll(
                    definitions.Select(c => c.RenderAsync(this, model, new RenderingContext(context.Writer, context))));
        }

        public async Task RenderModuleAsync(string moduleId, string skin, RenderingContext context)
        {
	        string dataVariation = null;
	        object dataVariationObj;
	        if (context.Data.TryGetValue("data_variation", out dataVariationObj))
				dataVariation = dataVariationObj as string;

            var moduleDefinition = await _moduleRepository.GetModuleDefinitionByIdAsync(moduleId).ConfigureAwait(false);
            if (moduleDefinition != null)
            {
                TemplateInfo templateInfo;
                if (string.IsNullOrEmpty(skin) || moduleDefinition.Skins == null || !moduleDefinition.Skins.TryGetValue(skin, out templateInfo))
                    templateInfo = moduleDefinition.DefaultTemplate;

                var view = await _viewEngine.CreateViewAsync(templateInfo).ConfigureAwait(false);
                if (view != null)
                {
                    var moduleModel = await _modelProvider.GetModelForModuleAsync(moduleDefinition, dataVariation).ConfigureAwait(false);
                    await view.RenderAsync(moduleModel, new RenderingContext(context.Writer, context));
                    return;
                }
            }

            await context.Writer.WriteAsync("Problem loading template " + moduleId + (!string.IsNullOrEmpty(skin) ? "-" + skin : string.Empty));
        }

		public Task RenderLabelAsync(string key, RenderingContext context)
	    {
		    return context.Writer.WriteAsync(_labelService.Get(key));
	    }

        public async Task RenderPartialAsync(string template, object model, RenderingContext context)
        {
            var templateInfo = await _templateRepository.GetTemplateAsync(template).ConfigureAwait(false);
            if (templateInfo != null)
            {
                var view = await _viewEngine.CreateViewAsync(templateInfo).ConfigureAwait(false);
                if (view != null)
                {
                    await view.RenderAsync(model, new RenderingContext(context.Writer, context));
                    return;
                }
            }

            await context.Writer.WriteAsync("Problem loading template " + template);
        }
    }
}