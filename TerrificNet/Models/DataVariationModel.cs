using Newtonsoft.Json;

namespace TerrificNet.Models
{
    public class DataVariationModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

		[JsonProperty("delete_link")]
	    public string DeleteLink { get; set; }

		[JsonProperty("edit_link")]
	    public string EditLink { get; set; }
    }
}