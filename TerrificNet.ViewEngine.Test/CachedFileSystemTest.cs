using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerrificNet.ViewEngine.IO;

namespace TerrificNet.ViewEngine.Test
{
	[TestClass]
	public class CachedFileSystemTest : FileSystemTest
	{
		[TestInitialize]
		public override void Init()
		{
			FileSystem = new FileSystem("", true);

			FileSystem.RemoveFile(TestFileName);
		}
	}
}
