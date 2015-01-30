using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerrificNet.ViewEngine.Config;
using TerrificNet.ViewEngine.Globalization;

namespace TerrificNet.ViewEngine.Test
{
	[TestClass]
	public class GlobalizationTest
	{
		private const string TestKey = "name";
		private const string TestPathKey = "person/male";

		private JsonLabelService _service;
		private FileSystem _fileSystem;
		private ITerrificNetConfig _terrificConfig;

		public TestContext TestContext { get; set; }

		[TestInitialize]
		public void Init()
		{
			_fileSystem = new FileSystem();
			_terrificConfig = new TerrificNetConfig { BasePath = "" };
			_service = new JsonLabelService(_fileSystem);
			_service.Remove(TestKey);
			_service.Remove(TestPathKey);
		}

		[TestMethod]
		public void TestRoot()
		{
			_service.Get(TestKey);
			_service.Set(TestKey, "xyz");

			var name = _service.Get(TestKey);
			Assert.AreEqual("xyz", name);

			_service.Remove(TestKey);
		}

		[TestMethod]
		public void TestPath()
		{
			_service.Get(TestPathKey);

			_service.Set(TestPathKey, "xyz");

			var name = _service.Get(TestPathKey);
			Assert.AreEqual("xyz", name);

			_service.Remove(TestPathKey);
		}

		[TestMethod]
		public void TestReload()
		{
			_service.Set(TestPathKey, "xyz");

			var service2 = new JsonLabelService(_fileSystem);
			var name = service2.Get(TestPathKey);
			Assert.AreEqual("xyz", name);
		}
	}
}
