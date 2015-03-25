using Newtonsoft.Json;
using TerrificNet.ViewEngine.TemplateHandler;

namespace TerrificNet.Models.PageEditor
{
	public class ElementEditorDefinition
	{
		[JsonProperty("html")]
		public string Html { get; set; }

		[JsonProperty("_placeholder")]
		public PlaceholderDefinitionCollection Placeholder { get; set; }
	}
}