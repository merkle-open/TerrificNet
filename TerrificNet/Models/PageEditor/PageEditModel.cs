using System.Collections.Generic;
using Newtonsoft.Json;

namespace TerrificNet.Models.PageEditor
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

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("layouts")]
        public IEnumerable<PageEditLayoutModel> Layouts { get; set; }
    }
}