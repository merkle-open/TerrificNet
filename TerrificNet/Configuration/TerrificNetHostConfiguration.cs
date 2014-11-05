using System.Collections.Generic;
using Newtonsoft.Json;
using TerrificNet.UnityModule;

namespace TerrificNet.Configuration
{
    public class TerrificNetHostConfiguration
    {
        [JsonProperty("applications")]
        public Dictionary<string, TerrificNetApplicationConfiguration> Applications { get; set; }
    }
}
