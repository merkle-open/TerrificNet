using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.ViewEngines;


namespace TerrificNet.Mvc
{
    public class MvcTerrificTemplateHandler : ITerrificTemplateHandler
    {
        public string RenderPlaceholder(object model, string key, RenderingContext context)
        {
            return "Placeholder for:" + key;
        }

        public string RenderModule(string templateName, string skin, RenderingContext context)
        {
            var mvcContext = context as MvcRenderingContext;
            if (mvcContext == null)
                throw new InvalidOperationException("MvcTerrificTemplateHandler can only be used inside a Mvc application.");

            var result = new HtmlHelper(mvcContext.ViewContext, mvcContext.ViewDataContainer).Action("Index", templateName);
            return result.ToHtmlString();
        }
    }
}