using Newtonsoft.Json;

namespace TerrificNet.ViewEngine.ViewEngines.TemplateHandler
{
    public class StyleImport
    {
        [JsonProperty("href")]
        public string Href { get; set; }
    }
}