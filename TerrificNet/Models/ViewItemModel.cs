using Newtonsoft.Json;

namespace TerrificNet.Models
{
    public class ViewItemModel
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("editUrl")]
        public string EditUrl { get; set; }
    }
}