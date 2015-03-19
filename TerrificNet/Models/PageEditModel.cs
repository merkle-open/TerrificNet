using System.Collections.Generic;
using Newtonsoft.Json;
using TerrificNet.ViewEngine.TemplateHandler;

namespace TerrificNet.Models
{
    public class PageEditModel
    {
        [JsonProperty("pageJson")]
        public string PageJson { get; set; }

        [JsonProperty("pageHtml")]
        public string PageHtml { get; set; }

        [JsonProperty("modules")]
        public IEnumerable<PageEditModuleModel> Modules { get; set; }

        [JsonProperty("app")]
        public string App { get; set; }
    }

    public class PageEditModuleModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("skins")]
        public IEnumerable<string> Skins { get; set; }
    }

    public class ModuleEditorDefinition
    {
        [JsonProperty("html")]
        public string Html { get; set; }
        [JsonProperty("_placeholder")]
        public PlaceholderDefinitionCollection Placeholder { get; set; }
    }
}