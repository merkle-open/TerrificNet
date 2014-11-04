using Newtonsoft.Json.Schema;

namespace TerrificNet.ViewEngine
{
    public interface ISchemaProvider
    {
        JsonSchema GetSchemaFromPath(string path);
    }
}