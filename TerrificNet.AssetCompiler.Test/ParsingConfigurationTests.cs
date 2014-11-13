using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using TerrificNet.AssetCompiler.Configuration;

namespace TerrificNet.AssetCompiler.Test
{
    [TestClass]
    public class ParsingConfigurationTests
    {
        [TestMethod]
        public void ParseValidJson()
        {
            const string file = "configs/valid.json";
            var config = TerrificConfig.Parse(file);
            Assert.IsNotNull(config);
            Assert.IsTrue(config.Assets.ContainsKey("app.css"));
            Assert.IsTrue(config.Assets.ContainsKey("app.js"));
        }

        [TestMethod]
        [ExpectedException(typeof(JsonReaderException))]
        public void ParseInvalidJson()
        {
            const string file = "configs/invalid.json";
            var config = TerrificConfig.Parse(file);
        }

        [TestMethod]
        public void ParseDefaultConfig()
        {
            var config = TerrificConfig.Parse();
            Assert.IsNotNull(config);
            Assert.IsTrue(config.Assets.ContainsKey("app.css"));
            Assert.IsTrue(config.Assets.ContainsKey("app.js"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseNullConfig()
        {
            var config = TerrificConfig.Parse(null);
        }
    }
}
