using Newtonsoft.Json;

namespace TerrificNet.ViewEngine.ViewEngines.TemplateHandler
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

        [JsonProperty("skin")]
        public string Skin { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }

    }
}