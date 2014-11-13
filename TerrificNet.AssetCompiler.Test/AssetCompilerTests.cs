using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TerrificNet.AssetCompiler.Configuration;

namespace TerrificNet.AssetCompiler.Test
{
    [TestClass]
    public class AssetCompilerTests
    {
        private AssetCompiler _compiler;

        [TestInitialize]
        public void PrepareAssetCompiler()
        {
            _compiler = new AssetCompiler();
        }

        [TestMethod]
        public void GetComponentForAssetJsTest()
        {
            var obj = new PrivateObject(_compiler);

            var js = _compiler.Configuration.Assets["app.js"];

            var jsFiles = obj.Invoke("GetComponentsForAsset", new object[] { js }) as AssetComponents;

            Assert.IsNotNull(jsFiles);
            Assert.AreEqual(jsFiles.Dependencies.Count, 0, "there were too few or too many files");
            Assert.AreEqual(jsFiles.Files.Count, 2, "there were too few or too many files");
            Assert.IsFalse(jsFiles.Files.Any(f => f.Contains("not-included.js")), "exclude was not recognized");
            Assert.IsTrue(jsFiles.Files.Any(f => f.Contains("example.js")), "asset was not recognized");
        }

        [TestMethod]
        public void GetComponentForAssetCssTest()
        {
            var obj = new PrivateObject(_compiler);

            var css = _compiler.Configuration.Assets["app.css"];

            var cssFiles = obj.Invoke("GetComponentsForAsset", new object[] { css }) as AssetComponents;

            Assert.IsNotNull(cssFiles);
            Assert.AreEqual(cssFiles.Dependencies.Count, 1, "there were too few or too many dependencies");
            Assert.AreEqual(cssFiles.Files.Count, 2, "there were too few or too many files");
            Assert.IsTrue(cssFiles.Dependencies.Any(f => f.Contains("variables.less")), "dependency was not recognized");
            Assert.IsFalse(cssFiles.Files.Any(f => f.Contains("variables.less")), "dependency was in files list");
            Assert.IsTrue(cssFiles.Files.Any(f => f.Contains("asset.css")), "asset was not recognized");
            Assert.IsTrue(cssFiles.Files.Any(f => f.Contains("example.less")), "asset was not recognized");
        }
    }
}
