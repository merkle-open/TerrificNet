using System.IO;
using Newtonsoft.Json;

namespace TerrificNet.Configuration
{
    static internal class TerrificNetHostConfigurationLoader
    {
        public static TerrificNetHostConfiguration LoadConfiguration(string path)
        {
            TerrificNetHostConfiguration configuration;
            using (var reader = new JsonTextReader(new StreamReader(path)))
            {
                configuration = new JsonSerializer().Deserialize<TerrificNetHostConfiguration>(reader);
            }

            foreach (var item in configuration.Applications)
            {
                item.Value.ApplicationName = item.Key;
            }

            return configuration;
        }
    }
}