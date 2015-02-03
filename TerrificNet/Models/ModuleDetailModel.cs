using System.Collections.Generic;
using Newtonsoft.Json;

namespace TerrificNet.Models
{
	public class ModuleDetailModel
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("default_template")]
		public TemplateItemModel DefaultTemplate { get; set; }

		[JsonProperty("skins")]
		public IList<TemplateItemModel> Skins { get; set; }
	}
}
