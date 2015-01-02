using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Nustache.Core;

namespace TerrificNet.ViewEngine.Helper
{
	public class TerrificPlaceholderHelper : IMustacheHelper
	{
		private readonly ITemplateRepository _templateRepository;
		private readonly IViewEngine _viewEngine;
		private readonly IModelProvider _modelProvider;

		public TerrificPlaceholderHelper(ITemplateRepository templateRepository, IViewEngine viewEngine, IModelProvider modelProvider)
		{
			_templateRepository = templateRepository;
			_viewEngine = viewEngine;
			_modelProvider = modelProvider;
		}

		public void Register(Func<string, bool> contains, Action<string, Nustache.Core.Helper> register)
		{
			if (!contains("placeholder"))
				register("placeholder", RenderPlaceholder);
		}

		private void RenderPlaceholder(RenderContext ctx, IList<object> args, IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
		{
			object rawKey;
			if (!options.TryGetValue("key", out rawKey))
				return;

			var key = rawKey as string;
			if (string.IsNullOrEmpty(key))
				return;

			var placeholderConfigs = ctx.GetValue("_placeholder." + key) as JArray;
			if (placeholderConfigs == null)
				return;

			foreach (var placeholderConfig in placeholderConfigs)
			{
				var templateName = placeholderConfig["template"].Value<string>();
				object model = placeholderConfig["data"];

				if (model == null)
					model = _modelProvider.GetModelForTemplate(templateName);

				TemplateInfo templateInfo;
				IView view;
				if (_templateRepository.TryGetTemplate(templateName, out templateInfo) && 
					_viewEngine.TryCreateView(templateInfo, out view))
				{
					ctx.Write(view.Render(model));
				}
				else
					ctx.Write("Problem loading template " + templateName);
			}
		}
	}
}