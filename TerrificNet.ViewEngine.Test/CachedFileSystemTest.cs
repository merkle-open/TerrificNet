using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TerrificNet.ViewEngine.Test
{
	[TestClass]
	public class CachedFileSystemTest
	{
		private IFileSystem _fileSystem;

		[TestInitialize]
		public void Init()
		{
			// Cleanup previous tests
			foreach (var file in Directory.GetFiles("./", "*"))
			{
				Console.WriteLine(file);
			}

			_fileSystem = new CachedFileSystem();
		}

		[TestMethod]
		public void TestRead()
		{
		}
	}
}
