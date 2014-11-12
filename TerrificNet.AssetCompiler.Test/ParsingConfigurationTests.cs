using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace TerrificAssetLibrary.Tests
{
    [TestClass]
    public class ParsingConfigurationTests
    {
        [TestMethod]
        public void ParseValidJson()
        {
            const string file = "configs/valid.json";
            var config = Configuration.Configuration.ParseConfiguration(file);
            Assert.IsNotNull(config);
            Assert.IsTrue(config.Assets.ContainsKey("app.css"));
            Assert.IsTrue(config.Assets.ContainsKey("app.js"));
        }

        [TestMethod]
        [ExpectedException(typeof(JsonReaderException))]
        public void ParseInvalidJson()
        {
            const string file = "configs/invalid.json";
            var config = Configuration.Configuration.ParseConfiguration(file);
        }

        [TestMethod]
        public void ParseDefaultConfig()
        {
            var config = Configuration.Configuration.ParseConfiguration();
            Assert.IsNotNull(config);
            Assert.IsTrue(config.Assets.ContainsKey("app.css"));
            Assert.IsTrue(config.Assets.ContainsKey("app.js"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseNullConfig()
        {
            var config = Configuration.Configuration.ParseConfiguration(null);
        }
    }
}
