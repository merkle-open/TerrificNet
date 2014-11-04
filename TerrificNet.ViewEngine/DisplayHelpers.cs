using System.Collections.Generic;
using Nustache.Core;

namespace TerrificNet.ViewEngine
{
	public static class DisplayHelpers
	{
		public static void Register(IViewEngine viewEngine)
		{
			if (!Helpers.Contains("module"))
				Helpers.Register("module", new TerrificModule(viewEngine).TerrificModuleHelper);
		}
	}

	class TerrificModule
	{
		private readonly IViewEngine _viewEngine;

		public TerrificModule(IViewEngine viewEngine)
		{
			_viewEngine = viewEngine;
		}

		public void TerrificModuleHelper(RenderContext ctx, IList<object> args, IDictionary<string, object> options,
			RenderBlock fn, RenderBlock inverse)
		{
			object template;
			if (!options.TryGetValue("template", out template))
				return;

			var templateName = template as string;
			if (string.IsNullOrEmpty(templateName))
				return;

			IView view;
			if (_viewEngine.TryCreateViewFromPath(templateName, out view))
				ctx.Write(view.Render(new Dictionary<string, object> {{"test", "stefan 123"}}));
			else
				ctx.Write("Problem loading template " + templateName);
		}
	}
}