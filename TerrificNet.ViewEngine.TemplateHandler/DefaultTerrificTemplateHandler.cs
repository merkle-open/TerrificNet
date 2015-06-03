using System;
using Newtonsoft.Json;
using TerrificNet.ViewEngine.Globalization;
using TerrificNet.ViewEngine.TemplateHandler.UI;
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

		public virtual void RenderPlaceholder(object model, string key, int? index, RenderingContext context)
		{
			if (index.HasValue)
				key = string.Join("_", key, index);

			ViewDefinition definition;
			if (!context.TryGetData("siteDefinition", out definition))
				throw new InvalidOperationException("The context must contain a siteDefinition to use the placeholder helper.");

			if (definition.Placeholder == null)
				return;

			ViewDefinition[] definitions;
			if (!definition.Placeholder.TryGetValue(key, out definitions))
				return;

			foreach (var placeholderConfig in definitions)
			{
				// TODO: Move to view definition
				var ctx = new RenderingContext(context.Writer, context);
				ctx.Data["siteDefinition"] = placeholderConfig;

				placeholderConfig.Render(this, model, ctx);
			}
		}

		public virtual void RenderModule(string moduleId, string skin, RenderingContext context)
		{
			string dataVariation = null;
			object dataVariationObj;
			if (context.Data.TryGetValue("data_variation", out dataVariationObj))
				dataVariation = dataVariationObj as string;

			var moduleDefinition = _moduleRepository.GetModuleDefinitionByIdAsync(moduleId).Result;
			if (moduleDefinition != null)
			{
				TemplateInfo templateInfo;
				if (string.IsNullOrEmpty(skin) || moduleDefinition.Skins == null ||
					!moduleDefinition.Skins.TryGetValue(skin, out templateInfo))
					templateInfo = moduleDefinition.DefaultTemplate;

				var view = _viewEngine.CreateViewAsync(templateInfo).Result;
				if (view != null)
				{
					var moduleModel = _modelProvider.GetModelForModuleAsync(moduleDefinition, dataVariation).Result;
					if (context.Data.ContainsKey("siteDefinition") && context.Data.ContainsKey("short_module"))
						context.Data["siteDefinition"] = JsonConvert.DeserializeObject<ModuleViewDefinition>(JsonConvert.SerializeObject(moduleModel));

					view.Render(moduleModel, new RenderingContext(context.Writer, context));

					return;
				}
			}

			throw new ArgumentException("Problem loading template " + moduleId +
										(!string.IsNullOrEmpty(skin) ? "-" + skin : string.Empty));

		}

		public virtual void RenderLabel(string key, RenderingContext context)
		{
			context.Writer.Write(_labelService.Get(key));
		}

		public virtual void RenderPartial(string template, object model, RenderingContext context)
		{
			var templateInfo = _templateRepository.GetTemplateAsync(template).Result;
			if (templateInfo != null)
			{
				var view = _viewEngine.CreateViewAsync(templateInfo).Result;
				if (view != null)
				{
					view.Render(model, new RenderingContext(context.Writer, context));

					return;
				}
			}

			throw new ArgumentException("Problem loading template " + template);
		}
	}
}