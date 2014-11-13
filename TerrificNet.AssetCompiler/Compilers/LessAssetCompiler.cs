using dotless.Core;
using dotless.Core.configuration;
using System.Threading.Tasks;

namespace TerrificNet.AssetCompiler.Compilers
{
    public class LessAssetCompiler : IAssetCompiler
    {
        /// <summary>
        /// (Awaitable) Compiles content with the give configuration (files and minify flag).
        /// </summary>
        /// <param name="content">Content to Compile</param>
        /// <returns>string with compiled content</returns>
        public Task<string> Compile(string content)
        {
            return Task.FromResult(Less.Parse(content, new DotlessConfiguration(){ MinifyOutput = true}));
        }
    }
}
