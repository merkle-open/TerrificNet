using System.Collections.Generic;
using Newtonsoft.Json;

namespace TerrificNet.Host
{
    public class Configuration
    {
        [JsonProperty("workspace")]
        public ApplicationWorkspaceSettings Workspace { get; set; }

        [JsonProperty("projects")]
        public Dictionary<string, ApplicationSettings> Projects { get; set; }
    }
}
