using System.Collections.Generic;
using TerrificNet.ViewEngine.IO;

namespace TerrificNet.ViewEngine.Config
{
	public interface ITerrificNetConfig
	{
		PathInfo BasePath { get; }
        PathInfo ViewPath { get; }
        PathInfo ModulePath { get; }
        PathInfo AssetPath { get; }
        PathInfo DataPath { get; }
		bool Minify { get; }

        Dictionary<string, string[]> Assets { get; set; }
	}
}
