using System.Collections.Generic;
using Newtonsoft.Json;

namespace TerrificNet.Models
{
	public class ModuleDetailModel
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("schema_url")]
		public string SchemaUrl { get; set; }

		[JsonProperty("default_template")]
		public TemplateItemModel DefaultTemplate { get; set; }

		[JsonProperty("skins")]
		public IList<TemplateItemModel> Skins { get; set; }

		[JsonProperty("data_variations")]
		public IList<DataVariationModel> DataVariations { get; set; }
	}
}
