using Newtonsoft.Json;

namespace TerrificNet.ViewEngine.TemplateHandler
{
    public class StyleImport
    {
        [JsonProperty("href")]
        public string Href { get; set; }
    }
}