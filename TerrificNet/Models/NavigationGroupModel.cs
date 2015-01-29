using System.Collections.Generic;
using Newtonsoft.Json;
using TerrificNet.ViewEngine.ViewEngines.TemplateHandler;

namespace TerrificNet.Models
{
    public class NavigationGroupModel : ViewDefinition
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
    }
}
