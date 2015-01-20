using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
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
            var config = ConfigurationLoader.LoadTerrificConfiguration(Path.Combine(TestContext.DeploymentDirectory, "configs"), file);
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
            var config = ConfigurationLoader.LoadTerrificConfiguration(Path.Combine(TestContext.DeploymentDirectory, "configs"), fileName);
        }

        [TestMethod]
        [DeploymentItem("configs/config.json")]
        public void ParseDefaultConfig()
        {
            var config = ConfigurationLoader.LoadTerrificConfiguration(TestContext.DeploymentDirectory);
            Assert.IsNotNull(config);
            Assert.IsTrue(config.Assets.ContainsKey("app.css"));
            Assert.IsTrue(config.Assets.ContainsKey("app.js"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseNullConfig()
        {
            var config = ConfigurationLoader.LoadTerrificConfiguration(null);
        }
    }
}
