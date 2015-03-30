using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Veil;

namespace TerrificNet.ViewEngine.TemplateHandler.UI
{
    public class PageViewDefinition : PageViewDefinition<PageViewData>
    {
        public PageViewDefinition()
        {
        }

        public PageViewDefinition(TemplateInfo templateInfo)
            : base(templateInfo)
        {
        }
    }

    public class PageViewDefinition<T> : PartialViewDefinition<T>, IPageViewDefinition
    {
        public TemplateInfo TemplateInfo { get; set; }

        public PageViewDefinition()
        {
        }

        public PageViewDefinition(TemplateInfo templateInfo)
        {
            TemplateInfo = templateInfo;
        }

        public string Id { get; set; }

        public async Task RenderAsync(IViewEngine engine, StreamWriter writer)
        {
            var view = await engine.CreateViewAsync(TemplateInfo, typeof(T)).ConfigureAwait(false);

            var renderingContext = new RenderingContext(writer);
            renderingContext.Data["siteDefinition"] = this;
            await view.RenderAsync(this.Data, renderingContext);
        }
    }

    public class PageViewData
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("styles")]
        public List<StyleImport> Styles { get; set; }

        [JsonProperty("scripts")]
        public List<ScriptImport> Scripts { get; set; }
    }
}