using System;

namespace TerrificNet.ViewEngine.SchemaProviders
{
    public interface IModelTypeRepository
    {
        Type GetModelTypeFromTemplate(TemplateInfo template);
    }
}