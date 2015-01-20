using Newtonsoft.Json;

namespace TerrificNet.Configuration
{
    public class TerrificNetApplicationConfiguration
    {
        [JsonIgnore]
        public string ApplicationName { get; set; }

        public string BasePath { get; set; }
        public string Section { get; set; }
    }
}