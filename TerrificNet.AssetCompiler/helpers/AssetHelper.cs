using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TerrificNet.AssetCompiler.Configuration;

namespace TerrificNet.AssetCompiler.Helpers
{
	public class AssetHelper : IAssetHelper
	{
		public AssetComponents GetGlobComponentsForAsset(IEnumerable<string> assetPaths, string basePath)
		{
			var files = new List<string>();
			var excludes = new List<string>();
			var dependencies = new List<string>();

			foreach (var path in assetPaths)
			{
				if (path.StartsWith("!"))
				{
					excludes.AddRange(Glob.Glob.Expand(basePath + path.Substring(1)).Select(f => f.FullName));
				}
				else if (path.StartsWith("+"))
				{
					dependencies.AddRange(Glob.Glob.Expand(basePath + path.Substring(1)).Select(f => f.FullName));
				}
				else
				{
					files.AddRange(Glob.Glob.Expand(basePath + path).Select(f => f.FullName));
				}
			}

			excludes.AddRange(dependencies);

			var components = new AssetComponents();

			dependencies.ForEach(o => components.Dependencies.Add(() => ReadFile(o)));
			files.Except(excludes).ForEach(o => components.Files.Add(() => ReadFile(o)));

			return components;
		}

		private static async Task<string> ReadFile(string path)
		{
			using (var file = new StreamReader(new FileStream(path, FileMode.Open)))
			{
				return await file.ReadToEndAsync();
			}
		}
	}
}
