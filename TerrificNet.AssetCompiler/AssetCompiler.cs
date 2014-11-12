using System.Collections.Generic;
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

        public async Task Compile(string compileDirectory)
        {
            //compile all assets to a specific folder
        }

        public async Task Compile(KeyValuePair<string, string[]> asset)
        {
            //compile 1 asset file of config
        }

        public async Task Compile(KeyValuePair<string, string[]> asset, string compileDirectory)
        {
            //compile 1 asset file of config to a specific folder
        }

        public void ContinuousCompile()
        {
            //start filewatcher and compile files.
        }

        public void ContinuousCompile(string compileDirectory)
        {
            //start filewatcher and compile files to a specific folder
        }

        private AssetComponent GetComponentsForAsset(KeyValuePair<string, string[]> asset)
        {
            //get the files and "compile with each file" for an asset
            return null;
        }
    }
}
