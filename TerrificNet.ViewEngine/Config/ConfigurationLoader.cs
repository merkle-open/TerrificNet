using System;
using System.IO;
using Newtonsoft.Json;

namespace TerrificNet.ViewEngine.Config
{
    public static class ConfigurationLoader
    {
        public static ITerrificNetConfig LoadTerrificConfiguration(string basePath)
        {
            return LoadTerrificConfiguration(basePath, "config.json");
        }

        public static ITerrificNetConfig LoadTerrificConfiguration(string basePath, string fileName)
        {
            var configPath = Path.Combine(basePath, fileName);
            TerrificNetConfig config;
            using (var reader = new JsonTextReader(new StreamReader(configPath)))
            {
                config = new JsonSerializer().Deserialize<TerrificNetConfig>(reader);
            }

            if (!Path.IsPathRooted(basePath))
                basePath = Path.GetFullPath(basePath);

            config.BasePath = basePath;
            config.ViewPath = Path.Combine(basePath, GetDefaultValueIfNotSet(config.ViewPath, basePath, "views"));
            config.ModulePath = Path.Combine(basePath,
                GetDefaultValueIfNotSet(config.ModulePath, basePath, "components/modules"));
            config.AssetPath = Path.Combine(basePath, GetDefaultValueIfNotSet(config.AssetPath, basePath, "assets"));
            config.DataPath = Path.Combine(basePath, GetDefaultValueIfNotSet(config.DataPath, basePath, "project/data"));

            return config;
        }

        private static string GetDefaultValueIfNotSet(string value, string basePath, string defaultLocation)
        {
            if (String.IsNullOrEmpty(value))
                return Path.Combine(basePath, defaultLocation);

            return value;
        }
    }
}