using System.Collections.Generic;
using Newtonsoft.Json;

namespace TerrificNet.Models
{
    public class ViewOverviewModel
    {
        [JsonProperty("views")]
        public IList<ViewItemModel> Views { get; set; }
    }
}
