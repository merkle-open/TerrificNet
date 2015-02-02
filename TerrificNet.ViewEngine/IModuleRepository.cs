using System.Collections.Generic;

namespace TerrificNet.ViewEngine
{
    public interface IModuleRepository
    {
        IEnumerable<ModuleDefinition> GetAll();

        bool TryGetModuleDefinitionById(string id, out ModuleDefinition moduleDefinition);
    }
}
