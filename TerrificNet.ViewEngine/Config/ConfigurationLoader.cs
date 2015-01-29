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
            if (basePath == null)
                throw new ArgumentNullException("basePath");

            // TODO: check need
            //if (!Path.IsPathRooted(basePath))
            //    basePath = Path.GetFullPath(basePath);

            if (!fileSystem.DirectoryExists(basePath))
                throw new ConfigurationException(string.Format("The base path ('{0}') for the configuration doesn't exist.", basePath));

            var configPath = fileSystem.Path.Combine(basePath, fileName);
            if (!fileSystem.FileExists(configPath))
                throw new ConfigurationException(string.Format("Could not find configuration in path '{0}'.", configPath));

            TerrificNetConfig config;
            using (var reader = new JsonTextReader(new StreamReader(fileSystem.OpenRead(configPath))))
            {
                config = new JsonSerializer().Deserialize<TerrificNetConfig>(reader);
            }

            config.BasePath = basePath;
            config.ViewPath = fileSystem.Path.Combine(basePath, GetDefaultValueIfNotSet(config.ViewPath, fileSystem, basePath, "views"));
            config.ModulePath = fileSystem.Path.Combine(basePath,
                GetDefaultValueIfNotSet(config.ModulePath, fileSystem, basePath, "components", "modules"));
            config.AssetPath = fileSystem.Path.Combine(basePath, GetDefaultValueIfNotSet(config.AssetPath, fileSystem, basePath, "assets"));
            config.DataPath = fileSystem.Path.Combine(basePath, GetDefaultValueIfNotSet(config.DataPath, fileSystem, basePath, "project", "data"));

            return config;
        }

        private static string GetDefaultValueIfNotSet(string value, IFileSystem fileSystem, params string[] defaultLocation)
        {
            if (String.IsNullOrEmpty(value))
                return fileSystem.Path.Combine(defaultLocation);

            return value;
        }
    }
}