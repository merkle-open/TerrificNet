using System.Collections.Generic;
using System.Linq;
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

            var isPageEditor = context.Data.ContainsKey("pageEditor") && (bool) context.Data["pageEditor"];

            ViewDefinition[] definitions;

            if (!placeholder.TryGetValue(key, out definitions))
                return;

            if (isPageEditor)
                context.Writer.Write("<div class='plh' id='plh_"+key+"_start'>Placeholder \"" + key + "\" before</div>");

            if (!context.Data.ContainsKey("placeholders"))
            {
                context.Data.Add("placeholders", new List<string>{key});
            }
            else
            {
                (context.Data["placeholders"] as List<string>).Add(key);
            }

            foreach (var placeholderConfig in definitions)
            {
                placeholderConfig.Render(this, model, new RenderingContext(context.Writer, context));
            }

            (context.Data["placeholders"] as List<string>).Remove(key);

            if (isPageEditor)
                context.Writer.Write("<div class='plh' id='plh_" + key + "_end'>Placeholder \"" + key + "\" after</div>");
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

                // TODO: make async
                var view = _viewEngine.CreateViewAsync(templateInfo).Result;
                if (view != null)
                {
                    var renderEditDivs = context.Data.ContainsKey("pageEditor") && (bool)context.Data["pageEditor"] && context.Data.ContainsKey("placeholders") && (context.Data["placeholders"] as List<string>).Any(); ;
                    
                    if (renderEditDivs)
                        context.Writer.Write("<div class='plh module'>Module \"" + moduleId + "\" before <span class='btn-delete' data-toggle='tooltip' data-placement='top' title='Delete module.'><i class='glyphicon glyphicon-remove'></i></span></div>");

                    // TODO: make async
                    var moduleModel = _modelProvider.GetModelForModuleAsync(moduleDefinition, dataVariation).Result;
                    view.Render(moduleModel, new RenderingContext(context.Writer, context));
                    
                    if (renderEditDivs)
                        context.Writer.Write("<div class='plh module'>Module \"" + moduleId + "\" after</div>");
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
                // TODO: Use async
                var view = _viewEngine.CreateViewAsync(templateInfo).Result;
                if (view != null)
                {
                    view.Render(model, new RenderingContext(context.Writer, context));
                    return;
                }
            }
            context.Writer.Write("Problem loading template " + template);
        }
    }
}