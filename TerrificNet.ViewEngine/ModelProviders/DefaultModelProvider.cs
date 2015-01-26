using System.Collections.Generic;

namespace TerrificNet.ViewEngine.ModelProviders
{
    public class DefaultModelProvider : IModelProvider
    {
        private readonly IModelProvider _fallbackProvider;
        private readonly Dictionary<string, IModelProvider> _providers = new Dictionary<string,IModelProvider>(); 

        public DefaultModelProvider(IModelProvider fallbackProvider)
        {
            _fallbackProvider = fallbackProvider;
        }

        public void RegisterProviderForTemplate(TemplateInfo template, IModelProvider provider)
        {
            if (!_providers.ContainsKey(template.Id))
                _providers.Add(template.Id, provider);
        }

        public object GetDefaultModelForTemplate(TemplateInfo template)
        {
            var provider = GetProvider(template);
            return provider.GetDefaultModelForTemplate(template);
        }

        public void UpdateDefaultModelForTemplate(TemplateInfo template, object content)
        {
            var provider = GetProvider(template);
            provider.UpdateDefaultModelForTemplate(template, content);
        }

        public object GetModelForTemplate(TemplateInfo template, string dataId)
        {
            var provider = GetProvider(template);
            return provider.GetModelForTemplate(template, dataId);
        }

        private IModelProvider GetProvider(TemplateInfo template)
        {
            IModelProvider provider;
            if (!_providers.TryGetValue(template.Id, out provider))
                provider = _fallbackProvider;
            return provider;
        }

    }
}
