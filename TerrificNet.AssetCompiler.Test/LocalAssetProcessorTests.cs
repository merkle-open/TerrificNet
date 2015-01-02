using System.Linq;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerrificNet.AssetCompiler.Bundler;
using TerrificNet.AssetCompiler.Compiler;
using TerrificNet.AssetCompiler.Configuration;
using TerrificNet.AssetCompiler.Helpers;
using TerrificNet.AssetCompiler.Processors;

namespace TerrificNet.AssetCompiler.Test
{
	[TestClass]
	public class LocalAssetProcessorTests
	{
		private readonly TerrificConfig _terrificConfig = TerrificConfig.Parse();
		private UnityContainer _container;

		[TestInitialize]
		public void Init()
		{
			_container = new UnityContainer();
			_container.RegisterType<IAssetCompiler, JsAssetCompiler>("Js");
			_container.RegisterType<IAssetCompiler, LessAssetCompiler>("Css");
			_container.RegisterType<IAssetCompilerFactory, AssetCompilerFactory>();
			_container.RegisterType<IAssetBundler, DefaultAssetBundler>();
			_container.RegisterType<IAssetHelper, AssetHelper>();
			_container.RegisterType<IAssetProcessor, BuildAssetProcessor>();
		}

		[TestMethod]
		public async Task BundleJsTest()
		{
			var bundler = _container.Resolve<IAssetBundler>();
			var helper = _container.Resolve<IAssetHelper>();
			var components = helper.GetGlobComponentsForAsset(_terrificConfig.Assets["app.js"], "");
			var bundle = await bundler.BundleAsync(components);
			Assert.IsTrue(bundle.Contains("TestLongParamName"));
		}

		[TestMethod]
		public async Task BundleCssTest()
		{
			var bundler = _container.Resolve<IAssetBundler>();
			var helper = _container.Resolve<IAssetHelper>();
			var components = helper.GetGlobComponentsForAsset(_terrificConfig.Assets["app.css"], "");
			var bundle = await bundler.BundleAsync(components);
			Assert.IsTrue(bundle.Contains(".example-css"));
		}

		[TestMethod]
		public async Task CompileAppJsAssetTest()
		{
			var factory = _container.Resolve<IAssetCompilerFactory>();
			var compiler = factory.GetCompiler("app.js");

			var helper = _container.Resolve<IAssetHelper>();
			var components = helper.GetGlobComponentsForAsset(_terrificConfig.Assets["app.js"], "");
			var bundle = await new DefaultAssetBundler().BundleAsync(components);
			var compile = await compiler.CompileAsync(bundle);
			Assert.IsFalse(compile.Contains("TestLongParamName"));
		}

		[TestMethod]
		public async Task CompileAppCssAssetTest()
		{
			var factory = _container.Resolve<IAssetCompilerFactory>();
			var compiler = factory.GetCompiler("app.css");

			var helper = _container.Resolve<IAssetHelper>();
			var components = helper.GetGlobComponentsForAsset(_terrificConfig.Assets["app.css"], "");
			var bundle = await new DefaultAssetBundler().BundleAsync(components);
			var compile = await compiler.CompileAsync(bundle);
			Assert.IsTrue(compile.Contains(".mod-example{background:#000}"));
		}

		[TestMethod]
		public void AssetCompilerFactoryJsTest()
		{
			var factory = _container.Resolve<IAssetCompilerFactory>();

			var compiler = factory.GetCompiler("app.js");
			Assert.IsInstanceOfType(compiler, typeof(JsAssetCompiler));
		}

		[TestMethod]
		public void AssetCompilerFactoryCssTest()
		{
			var factory = _container.Resolve<IAssetCompilerFactory>();

			var compiler = factory.GetCompiler("app.css");
			Assert.IsInstanceOfType(compiler, typeof(LessAssetCompiler));
		}

		[TestMethod]
		public async Task BuildAssetProcessJsWithoutMinifyTest()
		{
			var processor = _container.Resolve<IAssetProcessor>();
			var asset = _terrificConfig.Assets.First(o => o.Key == "app.js");
			var processed = await processor.ProcessAsync(asset.Key, asset.Value, ProcessorFlags.None, "");
			Assert.IsTrue(processed.Contains("TestLongParamName"));
		}

		[TestMethod]
		public async Task BuildAssetProcessCssWithoutMinifyTest()
		{
			var processor = _container.Resolve<IAssetProcessor>();
			var asset = _terrificConfig.Assets.First(o => o.Key == "app.css");
			var processed = await processor.ProcessAsync(asset.Key, asset.Value, ProcessorFlags.None, "");
			Assert.IsTrue(processed.Contains(".example-css"));
		}

		[TestMethod]
		public async Task BuildAssetProcessJsWithMinifyTest()
		{
			var processor = _container.Resolve<IAssetProcessor>();
			var asset = _terrificConfig.Assets.First(o => o.Key == "app.js");
			var processed = await processor.ProcessAsync(asset.Key, asset.Value, ProcessorFlags.Minify, "");
			Assert.IsFalse(processed.Contains("TestLongParamName"));
		}

		[TestMethod]
		public async Task BuildAssetProcessCssWithMinifyTest()
		{
			var processor = _container.Resolve<IAssetProcessor>();
			var asset = _terrificConfig.Assets.First(o => o.Key == "app.css");
			var processed = await processor.ProcessAsync(asset.Key, asset.Value, ProcessorFlags.Minify, "");
			Assert.IsTrue(processed.Contains(".mod-example{background:#000}"));
		}
	}
}
