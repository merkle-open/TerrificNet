using System.Threading.Tasks;
using Newtonsoft.Json.Schema;

namespace TerrificNet.ViewEngine
{
    public interface IModuleSchemaProvider
    {
        Task<JSchema> GetSchemaFromModuleAsync(ModuleDefinition module);
    }
}