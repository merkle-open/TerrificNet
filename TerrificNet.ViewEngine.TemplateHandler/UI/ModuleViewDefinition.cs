using System.Threading.Tasks;
using Newtonsoft.Json;
using Veil;

namespace TerrificNet.ViewEngine.TemplateHandler.UI
{
    public class ModuleViewDefinition : ViewDefinition
    {
        [JsonProperty("module")]
        public string Module { get; set; }

        [JsonProperty("skin")]
        public string Skin { get; set; }

        [JsonProperty("data_variation")]
        public string DataVariation { get; set; }

		protected internal override void Render(ITerrificTemplateHandler templateHandler, object model, RenderingContext context)
		{
			if (context.Data.ContainsKey("data_variation"))
				context.Data["data_variation"] = this.DataVariation;
			else
				context.Data.Add("data_variation", this.DataVariation);

			templateHandler.RenderModule(Module, Skin, context);
		}
    }
}