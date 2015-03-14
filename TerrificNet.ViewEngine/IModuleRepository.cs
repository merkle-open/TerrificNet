using System.Collections.Generic;
using System.Threading.Tasks;

namespace TerrificNet.ViewEngine
{
    public interface IModuleRepository
    {
        IEnumerable<ModuleDefinition> GetAll();

        Task<ModuleDefinition> GetModuleDefinitionByIdAsync(string id);
    }
}
