using System;
using System.IO;
using Microsoft.Ajax.Utilities;
using System.Threading.Tasks;

namespace TerrificNet.AssetCompiler.Compiler
{
	public class JsAssetCompiler : IAssetCompiler
	{
	    /// <summary>
	    /// (Awaitable) Compiles content with the give configuration (files and minify flag).
	    /// </summary>
	    /// <param name="content">Content to Compile</param>
	    /// <param name="minify"></param>
	    /// <returns>string with compiled content</returns>
	    public Task<string> CompileAsync(string content, bool minify)
	    {
	        if (minify)
	        {
	            var minifier = new Minifier();
	            return Task.FromResult(minifier.MinifyJavaScript(content));
	        }
	        return Task.FromResult(content);
	    }

	    public bool CanProcess(string filename)
		{
			return Path.HasExtension(filename) && ".js".Equals(Path.GetExtension(filename), StringComparison.OrdinalIgnoreCase);
		}

		public string MimeType
		{
			get { return "text/javascript"; }
		}
	}
}
