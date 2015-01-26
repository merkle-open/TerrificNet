using Newtonsoft.Json;

namespace TerrificNet.ViewEngine.ViewEngines.TemplateHandler
{
    public class PlaceholderDefinition : ViewDefinition
    {
        //[JsonProperty("template")]
        //public string Template { get; set; }

        [JsonProperty("skin")]
        public string Skin { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }
    }
}