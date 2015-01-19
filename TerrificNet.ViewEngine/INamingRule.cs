using Newtonsoft.Json.Schema;

namespace TerrificNet.ViewEngine
{
    public interface INamingRule
    {
        string GetClassName(JsonSchema schema, string propertyName);
        string GetClassNameFromArrayItem(JsonSchema schema, string propertyName);
        string GetPropertyName(string input);
        string GetNamespaceName(JsonSchema schema);
    }
}