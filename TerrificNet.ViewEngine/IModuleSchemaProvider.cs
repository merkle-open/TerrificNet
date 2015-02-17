using Newtonsoft.Json.Schema;

namespace TerrificNet.ViewEngine
{
    public interface IModuleSchemaProvider
    {
        JSchema GetSchemaFromModule(ModuleDefinition module);
    }
}