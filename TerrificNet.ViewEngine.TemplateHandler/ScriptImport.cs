using Newtonsoft.Json;

namespace TerrificNet.ViewEngine.TemplateHandler
{
    public class ScriptImport
    {
        [JsonProperty("src")]
        public string Src { get; set; }
    }
}