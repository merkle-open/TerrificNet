using System.Collections.Generic;

namespace TerrificNet.ViewEngine.Config
{
	public class TerrificNetConfig
	{
	    public string ViewPath { get; set; }
		public string ModulePath { get; set; }
		public string AssetPath { get; set; }
		public string DataPath { get; set; }

	    public Dictionary<string, string[]> Assets { get; set; }
	}
}