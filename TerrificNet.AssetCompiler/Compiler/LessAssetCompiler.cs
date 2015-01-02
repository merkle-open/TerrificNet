using System;
using System.IO;
using dotless.Core;
using dotless.Core.configuration;
using System.Threading.Tasks;

namespace TerrificNet.AssetCompiler.Compiler
{
	public class LessAssetCompiler : IAssetCompiler
	{
		/// <summary>
		/// (Awaitable) Compiles content with the give configuration (files and minify flag).
		/// </summary>
		/// <param name="content">Content to Compile</param>
		/// <returns>string with compiled content</returns>
		public Task<string> CompileAsync(string content)
		{
			return Task.FromResult(Less.Parse(content, new DotlessConfiguration { MinifyOutput = true }));
		}

		public bool CanProcess(string filename)
		{
			return Path.HasExtension(filename) && ".css".Equals(Path.GetExtension(filename), StringComparison.OrdinalIgnoreCase);
		}

		public string MimeType
		{
			get { return "text/css"; }
		}
	}
}
