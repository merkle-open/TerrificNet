using Newtonsoft.Json;
using Veil;

namespace TerrificNet.ViewEngine.TemplateHandler.UI
{
    public class PartialViewDefinition : PartialViewDefinition<object>
    {
    }

    public class PartialViewDefinition<T> : ViewDefinition, IPartialViewDefinition
    {
        [JsonProperty("template")]
        public string Template { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }

        object IPartialViewDefinition.Data { get { return this.Data; } }
		
		protected internal override void Render(ITerrificTemplateHandler templateHandler, object model, RenderingContext context)
		{
			templateHandler.RenderPartial(Template, ((IPartialViewDefinition)this).Data ?? model, context);
		}
    }
}