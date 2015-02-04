using System.Collections.Generic;
using Newtonsoft.Json;

namespace TerrificNet.Models
{
    public class NavigationGroupModel
    {
        [JsonProperty("actions")]
        public List<ActionModel> Actions { get; set; }
    }

    public class ActionModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
