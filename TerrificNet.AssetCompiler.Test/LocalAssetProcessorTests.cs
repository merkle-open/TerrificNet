using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using TerrificNet.AssetCompiler.Bundler;
using TerrificNet.AssetCompiler.Compiler;
using TerrificNet.AssetCompiler.Configuration;
using TerrificNet.AssetCompiler.Helpers;

namespace TerrificNet.AssetCompiler.Test
{
    [TestClass]
    public class LocalAssetProcessorTests
    {
        private readonly TerrificConfig _terrificConfig = TerrificConfig.Parse();

        [TestMethod]
        public async Task BundleJsTest()
        {
            var bundler = new DefaultAssetBundler();
            var components = AssetHelper.GetGlobComponentsForAsset(_terrificConfig.Assets["app.js"]);
            var bundle = await bundler.BundleAsync(components);
            Assert.IsTrue(bundle.Contains("console.log(\"test function\");"));
        }

        [TestMethod]
        public async Task BundleCssTest()
        {
            var bundler = new DefaultAssetBundler();
            var components = AssetHelper.GetGlobComponentsForAsset(_terrificConfig.Assets["app.css"]);
            var bundle = await bundler.BundleAsync(components);
            Assert.IsTrue(bundle.Contains(".example-css"));
        }

        [TestMethod]
        public async Task CompileAppJsAssetTest()
        {
            var compiler = new JsAssetCompiler();
            var components = AssetHelper.GetGlobComponentsForAsset(_terrificConfig.Assets["app.js"]);
            var bundle = await new DefaultAssetBundler().BundleAsync(components);
            var compile = await compiler.CompileAsync(bundle);
            Assert.IsTrue(compile.Contains("\"test function\""));
        }

        [TestMethod]
        public async Task CompileAppCssAssetTest()
        {
            var compiler = new LessAssetCompiler();
            var components = AssetHelper.GetGlobComponentsForAsset(_terrificConfig.Assets["app.css"]);
            var bundle = await new DefaultAssetBundler().BundleAsync(components);
            var compile = await compiler.CompileAsync(bundle);
            Assert.IsTrue(compile.Contains(".mod-example{background:#000}"));
        }
    }
}
