using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using TerrificNet.ViewEngine.Globalization;
using Veil;

namespace TerrificNet.ViewEngine.TemplateHandler
{
    public class DefaultTerrificTemplateHandler : ITerrificTemplateHandler
    {
        private readonly ILabelService _labelService;
        private readonly IModelProvider _modelProvider;
        private readonly IModuleRepository _moduleRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly IViewEngine _viewEngine;

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

	        if (context.Data.ContainsKey("siteDefinition"))
	        {
		        definition = context.Data["siteDefinition"] as ViewDefinition;
	        }
	        else
	        {

		        if (tmp != null)
		        {
			        definition = ViewDefinition.FromJObject<ViewDefinition>(tmp);
		        }
		        else
		        {
			        definition = model as ViewDefinition;
		        }
	        }

	        if (definition == null || definition.Placeholder == null)
                return;

            var placeholder = definition.Placeholder;

            var isPageEditor = context.Data.ContainsKey("pageEditor") && (bool) context.Data["pageEditor"];

            ViewDefinition[] definitions;

            if (!placeholder.TryGetValue(key, out definitions))
                return;

            if (isPageEditor)
            {
                if (!context.Data.ContainsKey("placeholders"))
                {
                    context.Data.Add("placeholders", new List<string> {key});
                }
                else
                {
                    (context.Data["placeholders"] as List<string>).Add(key);
                }

                context.Writer.Write("<div class='plh start' id='plh_" +
                                     (context.Data["placeholders"] as List<string>).Aggregate("",
                                         (s, s1) => s += s1 + "/", s => s.Substring(0, s.Length - 1)) +
                                     "'>Placeholder \"" + key +
                                     "\" before</div>");
            }

            foreach (var placeholderConfig in definitions)
            {
	            var ctx = new RenderingContext(context.Writer, context);
	            ctx.Data["siteDefinition"] = placeholderConfig;

                placeholderConfig.Render(this, model, ctx);
            }

            if (isPageEditor)
            {
                context.Writer.Write("<div class='plh end' id='plh_" +
                                     (context.Data["placeholders"] as List<string>).Aggregate("",
                                         (s, s1) => s += s1 + "/", s => s.Substring(0, s.Length - 1)) +
                                     "'>Placeholder \"" + key +
                                     "\" after</div>");
                (context.Data["placeholders"] as List<string>).Remove(key);
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
                if (string.IsNullOrEmpty(skin) || moduleDefinition.Skins == null ||
                    !moduleDefinition.Skins.TryGetValue(skin, out templateInfo))
                    templateInfo = moduleDefinition.DefaultTemplate;

                // TODO: make async
                var view = _viewEngine.CreateViewAsync(templateInfo).Result;
                if (view != null)
                {
                    var renderEditDivs = context.Data.ContainsKey("pageEditor") && (bool) context.Data["pageEditor"] &&
                                         context.Data.ContainsKey("placeholders") &&
                                         (context.Data["placeholders"] as List<string>).Any();
                    var plhId = "";
                    if (renderEditDivs)
                    {
                        plhId = (context.Data["placeholders"] as List<string>).Aggregate("", (s, s1) => s += s1 + "/",
                            s => s.Substring(0, s.Length - 1));
                        context.Writer.Write("<div class='plh module start' data-module-id='" + moduleId +
                                             "' data-plh-id='" +
                                             plhId +
                                             "' data-index='" +
                                             Guid.NewGuid() + "'>Module \"" +
                                             moduleId +
                                             "\" before <span class='btn-delete' data-toggle='tooltip' data-placement='top' title='Delete module.'><i class='glyphicon glyphicon-remove'></i></span></div>");
                    }

                    // TODO: make async
                    var moduleModel = _modelProvider.GetModelForModuleAsync(moduleDefinition, dataVariation).Result;
                    view.Render(moduleModel, new RenderingContext(context.Writer, context));

                    if (renderEditDivs)
                        context.Writer.Write("<div class='plh module end' data-plh-id='" + plhId + "'>Module \"" +
                                             moduleId + "\" after</div>");
                    return;
                }
            }

            context.Writer.Write("Problem loading template " + moduleId +
                                 (!string.IsNullOrEmpty(skin) ? "-" + skin : string.Empty));
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
                    var renderDivs = context.Data.ContainsKey("pageEditor") && (bool) context.Data["pageEditor"] &&
                                     context.Data.ContainsKey("placeholders") &&
                                     (context.Data["placeholders"] as List<string>).Any();
                    var plhId = "";

                    if (renderDivs)
                    {
                        plhId = (context.Data["placeholders"] as List<string>).Aggregate("", (s, s1) => s += s1 + "/",
                            s => s.Substring(0, s.Length - 1));

                        context.Writer.Write("<div class='plh template start' data-template-id='" + template +
                                             "' data-plh-id='" +
                                             plhId + "' data-index='" +
                                             Guid.NewGuid() + "'>Partial Template \"" +
                                             template +
                                             "\" before <span class='btn-delete' data-toggle='tooltip' data-placement='top' title='Delete template.'><i class='glyphicon glyphicon-remove'></i></span></div>");
                    }

                    view.Render(model, new RenderingContext(context.Writer, context));

                    if (renderDivs)
                        context.Writer.Write("<div class='plh template end' data-plh-id='" + plhId +
                                             "'>Partial Template \"" + template + "\" after</div>");
                    return;
                }
            }
            context.Writer.Write("Problem loading template " + template);
        }
    }
}