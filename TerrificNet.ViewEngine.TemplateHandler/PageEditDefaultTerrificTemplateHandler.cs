using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TerrificNet.ViewEngine.Globalization;
using TerrificNet.ViewEngine.TemplateHandler.UI;
using Veil;

namespace TerrificNet.ViewEngine.TemplateHandler
{
    public class PageEditDefaultTerrificTemplateHandler : DefaultTerrificTemplateHandler
    {
        public PageEditDefaultTerrificTemplateHandler(IViewEngine viewEngine, IModelProvider modelProvider, ITemplateRepository templateRepository, ILabelService labelService, IModuleRepository moduleRepository) 
			: base(viewEngine, modelProvider, templateRepository, labelService, moduleRepository)
        {
        }

		public override void RenderPlaceholder(object model, string key, int? index, RenderingContext context)
        {
            var isPageEditor = context.GetData("pageEditor", () => false);
            var renderPath = context.GetData("renderPath", () => new List<string>());

            if (isPageEditor) 
            {
                renderPath.Add(key);
                context.Writer.Write("<div class='plh start' id='plh_" +
                                     GetRenderPath(context) +
                                     "'>Placeholder \"" + key +
                                     "\" before</div>");
            }

            base.RenderPlaceholder(model, key, index, context);

            if (isPageEditor)
            {
                context.Writer.Write("<div class='plh end' id='plh_" +
                                     GetRenderPath(context) +
                                     "'>Placeholder \"" + key +
                                     "\" after</div>");

                renderPath.Remove(key);
            }
        }

        public override void RenderModule(string moduleId, string skin, RenderingContext context)
        {
            var renderPath = context.GetData("renderPath", () => new List<string>());
            var isPageEditor = context.GetData("pageEditor", () => false);

            var modId = new Regex("/[^/]+$").Match(moduleId).Value.Substring(1);
            var path = "";
            if (isPageEditor)
            {
                renderPath.Add(modId);
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

            base.RenderModule(moduleId, skin, context);

            if (isPageEditor)
            {
                context.Writer.Write("<div class='plh module end' data-path='" + path + "' data-self='" + modId +
                                     "'>Module \"" +
                                     moduleId + "\" after</div>");

                renderPath.Remove(modId);
            }
        }

        public override void RenderPartial(string template, object model, RenderingContext context)
        {
            var renderPath = context.GetData("renderPath", () => new List<string>());
            var isPageEditor = context.GetData("pageEditor", () => false);

            var path = "";
            var templateId = new Regex("/[^/]+$").Match(template).Value.Substring(1);
            if (isPageEditor)
            {
                renderPath.Add(templateId);
                path = GetRenderPath(context);
                context.Writer.Write("<div class='plh template start' data-template-id='" + template +
                                     "' data-path='" +
                                     path + "' data-index='" +
                                     Guid.NewGuid() + "' data-self='" + templateId + "'>Partial Template \"" +
                                     template +
                                     "\" before <span class='btn-delete' data-toggle='tooltip' data-placement='top' title='Delete template.'><i class='glyphicon glyphicon-remove'></i></span></div>");
                
                var partial = model as PartialViewDefinition;
                if (partial != null)
                {
                    if (partial.Data == null) partial.Data = new JObject();
                }
            }

            base.RenderPartial(template, model, context);

            if (isPageEditor)
            {
                context.Writer.Write("<div class='plh template end' data-path='" + path +
                                     "' data-self='" + templateId + "'>Partial Template \"" + template +
                                     "\" after</div>");

                renderPath.Remove(templateId);
            }
        }

        private static string GetRenderPath(RenderingContext context)
        {
            var list = context.Data["renderPath"] as List<string>;
            return string.Join("/", list.ToArray());
            //return list != null ? list.Aggregate("", (s, s1) => s += s1 + "/", s => s.Substring(0, s.Length - 1)) : "";
        }
    }
}