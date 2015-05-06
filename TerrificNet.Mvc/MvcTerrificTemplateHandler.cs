using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using TerrificNet.ViewEngine.TemplateHandler;
using Veil;

namespace TerrificNet.Mvc
{
	public class MvcTerrificTemplateHandler : ITerrificTemplateHandler
	{
		public Task RenderPlaceholderAsync(object model, string key, int? index, RenderingContext context)
		{
			return context.Writer.WriteAsync("Placeholder for:" + key);
		}

		public void RenderPlaceholder(object model, string key, int? index, RenderingContext context)
		{
			context.Writer.Write("Placeholder for:" + key);
		}

		public Task RenderModuleAsync(string moduleId, string skin, RenderingContext context)
		{
			var mvcContext = context as MvcRenderingContext;
			if (mvcContext == null)
				throw new InvalidOperationException("MvcTerrificTemplateHandler can only be used inside a Mvc application.");

			new HtmlHelper(mvcContext.ViewContext, mvcContext.ViewDataContainer).RenderAction("Index", moduleId);
			return Task.FromResult(false);
		}

		public void RenderModule(string moduleId, string skin, RenderingContext context)
		{
			var mvcContext = context as MvcRenderingContext;
			if (mvcContext == null)
				throw new InvalidOperationException("MvcTerrificTemplateHandler can only be used inside a Mvc application.");

			new HtmlHelper(mvcContext.ViewContext, mvcContext.ViewDataContainer).RenderAction("Index", moduleId);
		}

		public Task RenderLabelAsync(string key, RenderingContext context)
		{
			throw new NotImplementedException();
		}

		public void RenderLabel(string key, RenderingContext context)
		{
			throw new NotImplementedException();
		}

		public Task RenderPartialAsync(string template, object model, RenderingContext context)
		{
			throw new NotImplementedException();
		}

		public void RenderPartial(string template, object model, RenderingContext context)
		{
			throw new NotImplementedException();
		}
	}
}