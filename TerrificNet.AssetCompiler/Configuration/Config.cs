using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace TerrificNet.AssetCompiler.Configuration
{
    public class Config
    {
        public Dictionary<string, string[]> Assets { get; set; }

        public static Config ParseConfiguration()
        {
            try
            {
                var defaultConfig = Glob.Glob.Expand("**/config.json").First();
                return ParseConfiguration(defaultConfig.FullName);
            }
            catch (InvalidOperationException e)
            {
                throw new FileNotFoundException("no default config.json found in: " + System.Reflection.Assembly.GetCallingAssembly().Location);
            }
        }

        public static Config ParseConfiguration(string filepath)
        {
            if(string.IsNullOrWhiteSpace(filepath)) throw new ArgumentNullException("filepath");
            var assemblyLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location);
            if (assemblyLocation == null) throw new DirectoryNotFoundException("assembly directory not found");
            var path = filepath.Contains(assemblyLocation) ? filepath : Path.Combine(assemblyLocation, filepath);
            var reader = new JsonTextReader(new StreamReader(path));
            var serializer = new JsonSerializer { NullValueHandling = NullValueHandling.Ignore };
            return serializer.Deserialize<Config>(reader);
        }
    }
}
