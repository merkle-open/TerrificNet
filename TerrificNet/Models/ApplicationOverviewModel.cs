using System.Collections.Generic;
using Newtonsoft.Json;

namespace TerrificNet.Models
{
    public class ApplicationOverviewModel
    {
        [JsonProperty("applications")]
        public IList<ViewOverviewModel> Applications { get; set; }        
    }
}