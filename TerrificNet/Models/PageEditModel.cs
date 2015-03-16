using Newtonsoft.Json;
using TerrificNet.ViewEngine.TemplateHandler;

namespace TerrificNet.Models
{
    public class PageEditModel
    {
        [JsonProperty("pageJson")]
        public string PageJson { get; set; }
    }
}