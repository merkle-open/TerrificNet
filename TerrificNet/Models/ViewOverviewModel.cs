using System.Collections.Generic;
using Newtonsoft.Json;

namespace TerrificNet.Models
{
    public class ViewOverviewModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("views")]
        public IList<ViewItemModel> Views { get; set; }

        [JsonProperty("modules")]
        public IList<string> Modules { get; set; }
    }
}
