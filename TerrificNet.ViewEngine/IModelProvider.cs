using System.Collections.Generic;

namespace TerrificNet.ViewEngine
{
    public interface IModelProvider
    {
        object GetDefaultModelForTemplate(TemplateInfo template);
        void UpdateDefaultModelForTemplate(TemplateInfo template, object content);

        object GetModelForTemplate(TemplateInfo template, string dataId);
        object GetModelForModule(ModuleDefinition moduleDefinition, string dataId);
	    void UpdateModelForModule(ModuleDefinition moduleDefinition, string dataId, object content);
	    IEnumerable<string> GetDataVariations(ModuleDefinition moduleDefinition);
    }
}