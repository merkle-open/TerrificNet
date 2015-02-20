using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.ViewEngines;
using TerrificNet.ViewEngine.ViewEngines.TemplateHandler;


namespace TerrificNet.Mvc
{
    public class MvcTerrificTemplateHandler : ITerrificTemplateHandler
    {
        public void RenderPlaceholder(object model, string key, RenderingContext context)
        {
            context.Writer.Write("Placeholder for:" + key);
        }

        public void RenderModule(string moduleId, string skin, RenderingContext context)
        {
            var mvcContext = context as MvcRenderingContext;
            if (mvcContext == null)
                throw new InvalidOperationException("MvcTerrificTemplateHandler can only be used inside a Mvc application.");

            new HtmlHelper(mvcContext.ViewContext, mvcContext.ViewDataContainer).RenderAction("Index", moduleId);
        }

	    public void RenderLabel(string key, RenderingContext context)
	    {
		    throw new NotImplementedException();
	    }

        public void RenderPartial(string template, object model, RenderingContext context)
        {
            throw new NotImplementedException();
        }
    }
}