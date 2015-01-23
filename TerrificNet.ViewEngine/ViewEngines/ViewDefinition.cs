using Newtonsoft.Json;

namespace TerrificNet.ViewEngine.ViewEngines
{
    public class ViewDefinition
    {
        [JsonProperty("template")]
        public string Template { get; set; }

        [JsonProperty("_placeholder")]
        public PlaceholderDefinitionCollection Placeholder { get; set; }
    }
}