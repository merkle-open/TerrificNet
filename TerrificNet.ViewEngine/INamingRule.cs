using Newtonsoft.Json.Schema;

namespace TerrificNet.ViewEngine
{
    public interface INamingRule
    {
        string GetClassName(JSchema schema, string propertyName);
        string GetClassNameFromArrayItem(JSchema schema, string propertyName);
        string GetPropertyName(string input);
        string GetNamespaceName(JSchema schema);
    }
}