using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TerrificNet.AssetCompiler.Configuration;

namespace TerrificNet.AssetCompiler
{
    public class AssetCompiler
    {
        //todo: caching for filewatcher
        //-> compile: compiles whole stuff
        //-> continuous: filewatcher with cache (only files changed are newly compiled and concatenated)
        //-> with source maps?

        public Config Configuration { get; private set; }
        public string CompileDirectory { get; set; }

        public AssetCompiler()
        {
            Configuration = Config.ParseConfiguration();
        }

        public AssetCompiler(string configPath)
        {
            Configuration = Config.ParseConfiguration(configPath);
        }

        public AssetCompiler(Config configuration)
        {
            Configuration = configuration;
        }

        public async Task Compile()
        {
            //compile all assets
        }

        /// <summary>
        /// (Awaitable) Compiles an asset file with the given asset key.
        /// </summary>
        /// <param name="assetKey">The asset key to compile</param>
        /// <exception cref="KeyNotFoundException">If the given assetkey is not found in the configuration</exception>
        /// <returns>Filepath as string to the compiled file</returns>
        public async Task<string> Compile(string assetKey)
        {
            if(!Configuration.Assets.ContainsKey(assetKey)) throw new KeyNotFoundException("no asset with the key " + assetKey + " found");
            return "";
        }

        public void ContinuousCompile()
        {
            //start filewatcher and compile files.
        }

        private AssetComponents GetComponentsForAsset(IEnumerable<string> assetPaths)
        {
            var files = new List<string>();
            var excludes = new List<string>();
            var dependencies = new List<string>();

            foreach (var path in assetPaths)
            {
                if (path.StartsWith("!"))
                {
                    excludes.AddRange(Glob.Glob.Expand(path.Substring(1)).Select(f => f.FullName));
                }
                else if (path.StartsWith("+"))
                {
                    dependencies.AddRange(Glob.Glob.Expand(path.Substring(1)).Select(f => f.FullName));
                }
                else
                {
                    files.AddRange(Glob.Glob.Expand(path).Select(f => f.FullName));
                }
            }

            excludes.AddRange(dependencies);

            return new AssetComponents
            {
                Dependencies = dependencies,
                Files = files.Except(excludes).ToList()
            };
        }
    }
}
