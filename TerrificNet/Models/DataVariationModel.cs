using Newtonsoft.Json;

namespace TerrificNet.Models
{
    public class DataVariationModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }
    }
}