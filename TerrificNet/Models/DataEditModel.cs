using System.Collections.Generic;
using Newtonsoft.Json;

namespace TerrificNet.Models
{
    public class DataEditModel
    {
		[JsonProperty("schema_url")]
		public string SchemaUrl { get; set; }

		[JsonProperty("data_url")]
		public string DataUrl { get; set; }

        [JsonProperty("save_action_id")]
        public string SaveActionId { get; set; }
    }
}
