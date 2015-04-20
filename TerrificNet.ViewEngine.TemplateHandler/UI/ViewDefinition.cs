using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Veil;

namespace TerrificNet.ViewEngine.TemplateHandler.UI
{
    public abstract class ViewDefinition
    {
        [JsonProperty("_placeholder")]
        public PlaceholderDefinitionCollection Placeholder { get; set; }

        [JsonExtensionData]
        public IDictionary<string, object> ExtensionData { get; set; }

		protected internal abstract Task RenderAsync(ITerrificTemplateHandler templateHandler, object model, RenderingContext context);
		protected internal abstract void Render(ITerrificTemplateHandler templateHandler, object model, RenderingContext context);
	}
}