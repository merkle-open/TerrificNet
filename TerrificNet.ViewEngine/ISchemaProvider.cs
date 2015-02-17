using Newtonsoft.Json.Schema;

namespace TerrificNet.ViewEngine
{
    public interface ISchemaProvider
    {
        JSchema GetSchemaFromTemplate(TemplateInfo template);
    }

    public interface ISchemaProviderFactory
    {
        ISchemaProvider Create();
    }
}