using Newtonsoft.Json;

namespace TerrificNet.ViewEngine.ViewEngines.TemplateHandler
{
    public class ScriptImport
    {
        [JsonProperty("src")]
        public string Src { get; set; }
    }
}