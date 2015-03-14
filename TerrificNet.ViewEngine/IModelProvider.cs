using System.Collections.Generic;
using System.Threading.Tasks;

namespace TerrificNet.ViewEngine
{
    public interface IModelProvider
    {
        Task<object> GetDefaultModelForTemplateAsync(TemplateInfo template);
        Task UpdateDefaultModelForTemplateAsync(TemplateInfo template, object content);

        Task<object> GetModelForTemplateAsync(TemplateInfo template, string dataId);
        Task<object> GetModelForModuleAsync(ModuleDefinition moduleDefinition, string dataId);
	    Task UpdateModelForModuleAsync(ModuleDefinition moduleDefinition, string dataId, object content);
	    IEnumerable<string> GetDataVariations(ModuleDefinition moduleDefinition);
    }
}