using Newtonsoft.Json;

namespace TerrificNet.Host
{
    public class ApplicationSettings
    {
        [JsonIgnore]
        public string Name { get; set; }

        [JsonProperty("packageSource")]
        public string PackageSource { get; set; }

        [JsonProperty("packageId")]
        public string PackageId { get; set; }
    }
}