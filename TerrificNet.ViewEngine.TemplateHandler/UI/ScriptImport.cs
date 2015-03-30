using Newtonsoft.Json;

namespace TerrificNet.ViewEngine.TemplateHandler.UI
{
    public class ScriptImport
    {
        [JsonProperty("src")]
        public string Src { get; set; }
    }
}