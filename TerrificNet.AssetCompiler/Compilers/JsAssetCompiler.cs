using Microsoft.Ajax.Utilities;
using System.Threading.Tasks;

namespace TerrificNet.AssetCompiler.Compilers
{
    public class JsAssetCompiler : IAssetCompiler
    {
        /// <summary>
        /// (Awaitable) Compiles content with the give configuration (files and minify flag).
        /// </summary>
        /// <param name="content">Content to Compile</param>
        /// <returns>string with compiled content</returns>
        public Task<string> Compile(string content)
        {
            var minifier = new Minifier();
            return Task.FromResult(minifier.MinifyJavaScript(content));
        }
    }
}
