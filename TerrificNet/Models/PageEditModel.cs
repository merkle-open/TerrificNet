using System.Collections.Generic;
using Newtonsoft.Json;

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
    }

    public class PageEditModuleModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("html")]
        public string Html { get; set; }
        [JsonProperty("skins")]
        public IEnumerable<SkinInfoModel> Skins { get; set; }
    }

    public class SkinInfoModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("html")]
        public string Html { get; set; }
    }
}