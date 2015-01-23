using Newtonsoft.Json;

namespace TerrificNet.Host
{
    public class ApplicationSettings
    {
        [JsonIgnore]
        public string Name { get; set; }

        [JsonProperty("packageId")]
        public string PackageId { get; set; }

        [JsonProperty("preservedFiles")]
        public string[] PreservedFiles { get; set; }
    }
}