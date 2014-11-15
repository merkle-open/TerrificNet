using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using TerrificNet.AssetCompiler.Bundler;
using TerrificNet.AssetCompiler.Compiler;
using TerrificNet.AssetCompiler.Configuration;
using TerrificNet.AssetCompiler.Helpers;

namespace TerrificNet.AssetCompiler.Test
{
    [TestClass]
    public class LocalAssetCompilerTests
    {
        private readonly TerrificConfig _terrificConfig = TerrificConfig.Parse();

        /*[TestMethod]
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
        }*/

        [TestMethod]
        public async Task CompileAppJsAssetTest()
        {
            var compiler = new JsAssetCompiler();
            var components = AssetHelper.GetGlobComponentsForAsset(_terrificConfig.Assets["app.js"]);
            var bundle = await new DefaultAssetBundler().BundleAsync(components);
            var compile = await compiler.CompileAsync(bundle);
            Assert.IsNotNull("");
        }

        [TestMethod]
        public async Task CompileAppCssAssetTest()
        {
            var compiler = new LessAssetCompiler();
            var components = AssetHelper.GetGlobComponentsForAsset(_terrificConfig.Assets["app.css"]);
            var bundle = await new DefaultAssetBundler().BundleAsync(components);
            var compile = await compiler.CompileAsync(bundle);
            Assert.IsNotNull("");
        }

        public void BLA()
        {
            var a = new JsAssetCompiler();
            var tc = TerrificConfig.Parse();
            var cc = new CompileConfig();
        }
    }
}
