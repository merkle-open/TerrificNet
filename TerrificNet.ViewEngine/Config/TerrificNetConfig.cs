using System.Collections.Generic;
using Newtonsoft.Json;

namespace TerrificNet.ViewEngine.Config
{
	public class TerrificNetConfig : ITerrificNetConfig
	{
	    [JsonIgnore]
	    public string BasePath { get; set; }
	    public string ViewPath { get; set; }
		public string ModulePath { get; set; }
		public string AssetPath { get; set; }
		public string DataPath { get; set; }

	    public Dictionary<string, string[]> Assets { get; set; }
	}
}