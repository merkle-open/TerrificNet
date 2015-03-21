using System;
using System.IO;
using Newtonsoft.Json;
using TerrificNet.ViewEngine.IO;

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

            if (!fileSystem.DirectoryExists(null))
                throw new ConfigurationException(string.Format("The base path for the configuration doesn't exist in {0}.", fileSystem.BasePath));

            var basePathInfo = PathInfo.Create(basePath);
            var configFile = fileSystem.Path.Combine(basePathInfo, PathInfo.Create(fileName));

            if (!fileSystem.FileExists(configFile))
                throw new ConfigurationException(string.Format("Could not find configuration in path '{0}' in {1}.", configFile, fileSystem.BasePath));

            TerrificNetConfig config;
            using (var reader = new JsonTextReader(new StreamReader(fileSystem.OpenRead(configFile))))
            {
                config = new JsonSerializer().Deserialize<TerrificNetConfig>(reader);
            }

            config.ViewPath = fileSystem.Path.Combine(basePathInfo, GetDefaultValueIfNotSet(config.ViewPath, fileSystem, basePathInfo, PathInfo.Create("views"))).ToString();
            config.ModulePath = fileSystem.Path.Combine(basePathInfo,
                GetDefaultValueIfNotSet(config.ModulePath, fileSystem, basePathInfo, PathInfo.Create("components"), PathInfo.Create("modules"))).ToString();
            config.AssetPath = fileSystem.Path.Combine(basePathInfo, GetDefaultValueIfNotSet(config.AssetPath, fileSystem, basePathInfo, PathInfo.Create("assets"))).ToString();
            config.DataPath = fileSystem.Path.Combine(basePathInfo, GetDefaultValueIfNotSet(config.DataPath, fileSystem, basePathInfo, PathInfo.Create("project"), PathInfo.Create("data"))).ToString();

            return config;
        }

        private static PathInfo GetDefaultValueIfNotSet(string value, IFileSystem fileSystem, params PathInfo[] defaultLocation)
        {
            if (String.IsNullOrEmpty(value))
                return fileSystem.Path.Combine(defaultLocation);

            return PathInfo.Create(value);
        }
    }
}