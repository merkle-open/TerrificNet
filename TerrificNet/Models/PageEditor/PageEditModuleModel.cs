using System.Collections.Generic;
using Newtonsoft.Json;

namespace TerrificNet.Models.PageEditor
{
	public class PageEditModuleModel
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("skins")]
		public IEnumerable<string> Skins { get; set; }

		[JsonProperty("variations")]
		public string Variations { get; set; }
	}
}