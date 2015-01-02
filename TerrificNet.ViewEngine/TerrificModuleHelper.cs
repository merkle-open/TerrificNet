using System;
using System.Collections.Generic;
using Nustache.Core;

namespace TerrificNet.ViewEngine
{
	public class TerrificModuleHelper : IMustacheHelper
	{
		private readonly IViewEngine _viewEngine;
		private readonly IModelProvider _modelProvider;
		private readonly ITemplateRepository _templateRepository;

		public TerrificModuleHelper(IViewEngine viewEngine, IModelProvider modelProvider, ITemplateRepository templateRepository)
		{
			_viewEngine = viewEngine;
			_modelProvider = modelProvider;
			_templateRepository = templateRepository;
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

			TemplateInfo templateInfo;
			IView view;
			if (_templateRepository.TryGetTemplate(templateName, out templateInfo) && _viewEngine.TryCreateView(templateInfo, out view))
			{
				var model = _modelProvider.GetModelFromPath(templateName);
				ctx.Write(view.Render(model));
			}
			else
				ctx.Write("Problem loading template " + templateName);
		}
	}
}