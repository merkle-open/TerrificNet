using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TerrificNet.ViewEngine.ViewEngines.TemplateHandler
{
    public class ViewDefinition
    {
        [JsonProperty("template")]
        public string Template { get; set; }

        [JsonProperty("_placeholder")]
        public PlaceholderDefinitionCollection Placeholder { get; set; }

        [JsonProperty("styles")]
        public List<StyleImport> Styles { get; set; }

        [JsonProperty("scripts")]
        public List<ScriptImport> Scripts { get; set; }

        [JsonProperty("skin")]
        public string Skin { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }

        [JsonExtensionData]
        public IDictionary<string, object> ExtensionData { get; set; }

    }
}