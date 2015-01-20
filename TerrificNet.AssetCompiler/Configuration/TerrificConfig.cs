using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TerrificNet.AssetCompiler.Helpers;

namespace TerrificNet.AssetCompiler.Configuration
{
    public class TerrificConfig
    {
        public Dictionary<string, string[]> Assets { get; set; }

        public static TerrificConfig Parse()
        {
            try
            {
                var defaultConfig = Glob.Glob.Expand("**/config.json").First();
                return Parse(defaultConfig.FullName);
            }
            catch (InvalidOperationException e)
            {
                throw new FileNotFoundException("no default config.json found in: " + PathHelper.AssemblyDirectory);
            }
        }

        public static TerrificConfig Parse(string filepath)
        {
            if(string.IsNullOrWhiteSpace(filepath)) 
                throw new ArgumentNullException("filepath");

            var path = filepath.Contains(PathHelper.AssemblyDirectory) ? filepath : Path.Combine(PathHelper.AssemblyDirectory, filepath);
            var reader = new JsonTextReader(new StreamReader(path));
            var serializer = new JsonSerializer { NullValueHandling = NullValueHandling.Ignore };
            return serializer.Deserialize<TerrificConfig>(reader);
        }
    }
}
