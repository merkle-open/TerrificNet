using Newtonsoft.Json;

namespace TerrificNet.Models.PageEditor
{
	public class PageEditLayoutModel
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }
	}
}