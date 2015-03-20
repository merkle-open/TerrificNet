using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
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

            if (context.Data.ContainsKey("layoutPlaceholders"))
            {
                var placeholders = context.Data["layoutPlaceholders"] as PlaceholderDefinitionCollection;
                if (placeholders != null)
                {
                    if (!placeholders.ContainsKey(key)) placeholders.Add(key, new ViewDefinition[0]);
                }
                if(definition != null) definition.Placeholder = placeholders;
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
                if (!context.Data.ContainsKey("renderPath"))
                {
                    context.Data.Add("renderPath", new List<string> {key});
                }
                else
                {
                    (context.Data["renderPath"] as List<string>).Add(key);
                }

                context.Writer.Write("<div class='plh start' id='plh_" +
                                     GetRenderPath(context) +
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
                                     GetRenderPath(context) +
                                     "'>Placeholder \"" + key +
                                     "\" after</div>");
                (context.Data["renderPath"] as List<string>).Remove(key);
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
                                         context.Data.ContainsKey("renderPath");
                    var modId = new Regex("/[^/]+$").Match(moduleId).Value.Substring(1);
                    var path = "";
                    if (renderEditDivs)
                    {
                        (context.Data["renderPath"] as List<string>).Add(modId);
                        path = GetRenderPath(context);
                        context.Writer.Write("<div class='plh module start' data-module-id='" + moduleId +
                                             "' data-path='" +
                                             path +
                                             "' data-self='" +
                                             modId +
                                             "' data-index='" +
                                             Guid.NewGuid() + "'>Module \"" +
                                             moduleId +
                                             "\" before <span class='btn-delete' data-toggle='tooltip' data-placement='top' title='Delete module.'><i class='glyphicon glyphicon-remove'></i></span></div>");
                    }

                    // TODO: make async
                    var moduleModel = _modelProvider.GetModelForModuleAsync(moduleDefinition, dataVariation).Result;
                    if (context.Data.ContainsKey("siteDefinition") && context.Data.ContainsKey("short_module")) context.Data["siteDefinition"] = JsonConvert.DeserializeObject<ModuleViewDefinition>(JsonConvert.SerializeObject(moduleModel));
                    view.Render(moduleModel, new RenderingContext(context.Writer, context));

                    if (renderEditDivs)
                    {
                        context.Writer.Write("<div class='plh module end' data-path='" + path + "' data-self='" + modId +
                                             "'>Module \"" +
                                             moduleId + "\" after</div>");
                        (context.Data["renderPath"] as List<string>).Remove(modId);
                    }
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
                                     context.Data.ContainsKey("renderPath") && (context.Data["renderPath"] as List<string>).Any();
                    var path = "";
                    var templateId = new Regex("/[^/]+$").Match(template).Value.Substring(1);
                    if (renderDivs)
                    {
                        (context.Data["renderPath"] as List<string>).Add(templateId);
                        path = GetRenderPath(context);
                        context.Writer.Write("<div class='plh template start' data-template-id='" + template +
                                             "' data-path='" +
                                             path + "' data-index='" +
                                             Guid.NewGuid() + "' data-self='"+templateId+"'>Partial Template \"" +
                                             template +
                                             "\" before <span class='btn-delete' data-toggle='tooltip' data-placement='top' title='Delete template.'><i class='glyphicon glyphicon-remove'></i></span></div>");
                        var partial = model as PartialViewDefinition;
                        if (partial != null)
                        {
                            if (partial.Data == null) partial.Data = new JObject();
                        }
                    }

                    view.Render(model, new RenderingContext(context.Writer, context));

                    if (renderDivs)
                    {
                        context.Writer.Write("<div class='plh template end' data-path='" + path +
                                             "' data-self='" + templateId + "'>Partial Template \"" + template +
                                             "\" after</div>");
                        (context.Data["renderPath"] as List<string>).Remove(templateId);
                    }
                    return;
                }
            }
            context.Writer.Write("Problem loading template " + template);
        }

        private static string GetRenderPath(RenderingContext context)
        {
            var list = context.Data["renderPath"] as List<string>;
            return list != null ? list.Aggregate("", (s, s1) => s += s1 + "/", s => s.Substring(0, s.Length - 1)) : "";
        }
    }
}