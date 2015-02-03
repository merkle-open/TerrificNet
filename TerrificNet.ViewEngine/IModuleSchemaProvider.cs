using Newtonsoft.Json.Schema;

namespace TerrificNet.ViewEngine
{
    public interface IModuleSchemaProvider
    {
        JsonSchema GetSchemaFromModule(ModuleDefinition module);
    }
}