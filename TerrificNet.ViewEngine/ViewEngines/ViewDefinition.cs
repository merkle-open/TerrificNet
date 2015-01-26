using Newtonsoft.Json;

namespace TerrificNet.ViewEngine.ViewEngines
{
    public class ViewDefinition
    {
        [JsonProperty("template")]
        public string Template { get; set; }

        [JsonProperty("_placeholder")]
        public PlaceholderDefinitionCollection Placeholder { get; set; }

        [JsonProperty("styles")]
        public StyleImport[] Styles { get; set; }

        [JsonProperty("scripts")]
        public ScriptImport[] Scripts { get; set; }
    }

    public class ScriptImport
    {
        [JsonProperty("src")]
        public string Src { get; set; }
    }

    public class StyleImport
    {
        [JsonProperty("href")]
        public string Href { get; set; }
    }
}