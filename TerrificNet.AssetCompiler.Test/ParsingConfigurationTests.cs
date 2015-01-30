using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.Config;

namespace TerrificNet.AssetCompiler.Test
{
    [TestClass]
    public class ParsingConfigurationTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DeploymentItem("configs/valid.json", "configs")]
        public void ParseValidJson()
        {
            const string file = "valid.json";
            var config = ConfigurationLoader.LoadTerrificConfiguration(string.Empty, file, new FileSystem(Path.Combine(TestContext.DeploymentDirectory, "configs")));
            Assert.IsNotNull(config);
            Assert.IsTrue(config.Assets.ContainsKey("app.css"));
            Assert.IsTrue(config.Assets.ContainsKey("app.js"));
        }

        [TestMethod]
        [ExpectedException(typeof(JsonReaderException))]
        [DeploymentItem("configs/invalid.json", "configs")]
        public void ParseInvalidJson()
        {
            const string fileName = "invalid.json";
            var config = ConfigurationLoader.LoadTerrificConfiguration(string.Empty, fileName, new FileSystem(Path.Combine(TestContext.DeploymentDirectory, "configs")));
        }

        [TestMethod]
        [DeploymentItem("configs/config.json")]
        public void ParseDefaultConfig()
        {
            var config = ConfigurationLoader.LoadTerrificConfiguration(string.Empty, new FileSystem(TestContext.DeploymentDirectory));
            Assert.IsNotNull(config);
            Assert.IsTrue(config.Assets.ContainsKey("app.css"));
            Assert.IsTrue(config.Assets.ContainsKey("app.js"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseNullConfig()
        {
            var config = ConfigurationLoader.LoadTerrificConfiguration(null, new FileSystem());
        }
    }
}
