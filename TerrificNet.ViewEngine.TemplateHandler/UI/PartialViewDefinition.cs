using System.Threading.Tasks;
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

        protected internal override Task RenderAsync(ITerrificTemplateHandler templateHandler, object model, RenderingContext context)
        {
            return templateHandler.RenderPartialAsync(Template, ((IPartialViewDefinition)this).Data ?? model, context);
        }
    }
}