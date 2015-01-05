using Newtonsoft.Json.Schema;

namespace TerrificNet.ViewEngine
{
    public interface ISchemaProvider
    {
        JsonSchema GetSchemaFromTemplate(TemplateInfo template);
    }
}