using Newtonsoft.Json;
using TerrificNet.ViewEngine.TemplateHandler;

namespace TerrificNet.Models
{
    public class PageEditModel
    {
        [JsonProperty("pageJson")]
        public string PageJson { get; set; }

		[JsonProperty("html")]
	    public string Html { get; set; }
    }
}