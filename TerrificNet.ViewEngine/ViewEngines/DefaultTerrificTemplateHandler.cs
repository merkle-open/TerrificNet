using System.Text;
using Newtonsoft.Json.Linq;

namespace TerrificNet.ViewEngine.ViewEngines
{
    public class DefaultTerrificTemplateHandler : ITerrificTemplateHandler
    {
        private readonly IViewEngine _viewEngine;
        private readonly IModelProvider _modelProvider;
        private readonly ITemplateRepository _templateRepository;

        public DefaultTerrificTemplateHandler(IViewEngine viewEngine, IModelProvider modelProvider,
            ITemplateRepository templateRepository)
        {
            _viewEngine = viewEngine;
            _modelProvider = modelProvider;
            _templateRepository = templateRepository;
        }

        public string RenderPlaceholder(object model, string key, RenderingContext context)
        {
            var tmp = model as JObject;
            if (tmp == null)
                return string.Empty;

            var placeholder = tmp.GetValue("_placeholder") as JObject;
            if (placeholder == null)
                return string.Empty;

            var placeholderConfigs = placeholder.GetValue(key) as JArray;
            if (placeholderConfigs == null)
                return string.Empty;

            var sb = new StringBuilder();
            foreach (var placeholderConfig in placeholderConfigs)
            {
                var templateName = placeholderConfig["template"].Value<string>();

                var skin = string.Empty;
                var skinRaw = placeholderConfig["skin"];
                if (skinRaw != null)
                    skin = placeholderConfig["skin"].Value<string>();

                TemplateInfo templateInfo;
                IView view;
                if (_templateRepository.TryGetTemplate(templateName, skin, out templateInfo) &&
                    _viewEngine.TryCreateView(templateInfo, out view))
                {
                    var moduleModel = placeholderConfig["data"] ?? _modelProvider.GetModelForTemplate(templateInfo);
                    sb.Append(view.Render(moduleModel, context));
                }
                else
                    sb.Append("Problem loading template " + templateName +
                              (!string.IsNullOrEmpty(skin) ? "-" + skin : string.Empty));
            }

            return sb.ToString();
        }

        public string RenderModule(string templateName, string skin, RenderingContext context)
        {
            TemplateInfo templateInfo;
            IView view;
            if (_templateRepository.TryGetTemplate(templateName, skin, out templateInfo) &&
                _viewEngine.TryCreateView(templateInfo, out view))
            {
                var moduleModel = _modelProvider.GetModelForTemplate(templateInfo) ?? new object();
                return view.Render(moduleModel, context);
            }

            return "Problem loading template " + templateName + (!string.IsNullOrEmpty(skin) ? "-" + skin : string.Empty);
        }
    }
}