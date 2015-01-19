using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.ViewEngines;


namespace TerrificNet.Mvc
{
    public class MvcTerrificTemplateHandler : ITerrificTemplateHandler
    {
        public void RenderPlaceholder(object model, string key, RenderingContext context)
        {
            context.Writer.Write("Placeholder for:" + key);
        }

        public void RenderModule(string templateName, string skin, RenderingContext context)
        {
            var mvcContext = context as MvcRenderingContext;
            if (mvcContext == null)
                throw new InvalidOperationException("MvcTerrificTemplateHandler can only be used inside a Mvc application.");

            new HtmlHelper(mvcContext.ViewContext, mvcContext.ViewDataContainer).RenderAction("Index", templateName);
        }
    }
}