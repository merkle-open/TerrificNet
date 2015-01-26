using System;
using System.IO;
using Newtonsoft.Json;

namespace TerrificNet.ViewEngine.Config
{
    public static class ConfigurationLoader
    {
        public static ITerrificNetConfig LoadTerrificConfiguration(string basePath, IFileSystem fileSystem)
        {
            return LoadTerrificConfiguration(basePath, "config.json", fileSystem);
        }

        public static ITerrificNetConfig LoadTerrificConfiguration(string basePath, string fileName, IFileSystem fileSystem)
        {
            // TODO: check need
            //if (!Path.IsPathRooted(basePath))
            //    basePath = Path.GetFullPath(basePath);

            if (!fileSystem.DirectoryExists(basePath))
                throw new ConfigurationException(string.Format("The base path ('{0}') for the configuration doesn't exist.", basePath));

            var configPath = Path.Combine(basePath, fileName);
            if (!fileSystem.FileExists(configPath))
                throw new ConfigurationException(string.Format("Could not find configuration in path '{0}'.", configPath));

            TerrificNetConfig config;
            using (var reader = new JsonTextReader(fileSystem.OpenRead(configPath)))
            {
                config = new JsonSerializer().Deserialize<TerrificNetConfig>(reader);
            }

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