using System;

namespace TerrificNet.ViewEngine
{
    public interface IModelTypeProvider
    {
        bool TryGetModelTypeFromTemplate(TemplateInfo templateInfo, out Type type);
    }
}
