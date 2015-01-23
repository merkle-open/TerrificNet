using System;

namespace TerrificNet.ViewEngine.ModelProviders
{
    public class DefaultModelProvider : IModelProvider
    {
        private readonly IModelProvider[] _modelProviders;

        public DefaultModelProvider(IModelProvider[] modelProviders)
        {
            _modelProviders = modelProviders;
        }

        public object GetDefaultModelForTemplate(TemplateInfo template)
        {
            foreach (var provider in _modelProviders)
            {
                var model = provider.GetDefaultModelForTemplate(template);
                if (model != null)
                    return model;
            }

            return null;
        }

        public void UpdateDefaultModelForTemplate(TemplateInfo template, object content)
        {
            throw new NotImplementedException();
        }

        public object GetModelForTemplate(TemplateInfo template, string dataId)
        {
            throw new NotImplementedException();
        }
    }
}
