using System;
using System.Threading.Tasks;

namespace TerrificNet.ViewEngine
{
    public interface IModelTypeProvider
    {
        Task<Type> GetModelTypeFromTemplateAsync(TemplateInfo templateInfo);
    }
}
