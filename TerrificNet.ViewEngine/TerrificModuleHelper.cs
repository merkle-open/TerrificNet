using System;
using System.Collections.Generic;
using Nustache.Core;

namespace TerrificNet.ViewEngine
{
	public class TerrificModuleHelper : IMustacheHelper
	{
		private readonly IViewEngine _viewEngine;
		private readonly IModelProvider _modelProvider;
		private readonly ITemplateLocator _templateLocator;

		public TerrificModuleHelper(IViewEngine viewEngine, IModelProvider modelProvider, ITemplateLocator templateLocator)
		{
			_viewEngine = viewEngine;
			_modelProvider = modelProvider;
			_templateLocator = templateLocator;
		}

		public void Register(Func<string, bool> contains, Action<string, Helper> register)
		{
			if (!contains("module"))
			{
				register("module", TerrificModule);
			}
		}

		private void TerrificModule(RenderContext ctx, IList<object> args, IDictionary<string, object> options,
			RenderBlock fn, RenderBlock inverse)
		{
			object template;
			if (!options.TryGetValue("template", out template))
				return;

			var templateName = template as string;
			if (string.IsNullOrEmpty(templateName))
				return;

			string modulePath;
			IView view;
			if (_templateLocator.TryLocateTemplate(templateName, out modulePath) && _viewEngine.TryCreateViewFromPath(modulePath, out view))
			{
				var model = _modelProvider.GetModelFromPath(templateName);
				ctx.Write(view.Render(model));
			}
			else
				ctx.Write("Problem loading template " + templateName);
		}
	}
}