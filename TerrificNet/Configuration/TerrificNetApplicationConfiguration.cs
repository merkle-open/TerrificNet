using Newtonsoft.Json;

namespace TerrificNet.Configuration
{
    public class TerrificNetApplicationConfiguration : TerrificNetConfig
    {
        [JsonProperty("section")]
        public string Section { get; set; }
    }
}