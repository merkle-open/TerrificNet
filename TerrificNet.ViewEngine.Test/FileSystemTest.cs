using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerrificNet.ViewEngine.IO;

namespace TerrificNet.ViewEngine.Test
{
	[TestClass]
	public class FileSystemTest
	{
        protected PathInfo TestFileName = PathInfo.Create("test.txt");
		protected const string TestFilePattern = "*.txt";
		protected IFileSystem FileSystem;

		[TestInitialize]
		public virtual void Init()
		{
			FileSystem = new FileSystem();
			FileSystem.RemoveFile(TestFileName);
		}

		[TestMethod]
		public void TestWrite()
		{
			Assert.AreEqual(false, FileSystem.FileExists(TestFileName));

			using (var stream = new StreamWriter(FileSystem.OpenWrite(TestFileName)))
			{
				stream.Write("123456");
			}

			Assert.AreEqual(true, FileSystem.FileExists(TestFileName));

			FileSystem.RemoveFile(TestFileName);
		}

		[TestMethod]
		public void TestReWrite()
		{
			Assert.AreEqual(false, FileSystem.FileExists(TestFileName));

			using (var stream = new StreamWriter(FileSystem.OpenWrite(TestFileName)))
			{
				stream.Write("123456");
			}

			Assert.AreEqual(true, FileSystem.FileExists(TestFileName));

			using (var stream = new StreamReader(FileSystem.OpenRead(TestFileName)))
			{
				Assert.AreEqual("123456", stream.ReadToEnd());
			}

			using (var stream = new StreamWriter(FileSystem.OpenWrite(TestFileName)))
			{
				stream.Write("654321");
			}

			Assert.AreEqual(true, FileSystem.FileExists(TestFileName));

			using (var stream = new StreamReader(FileSystem.OpenRead(TestFileName)))
			{
				Assert.AreEqual("654321", stream.ReadToEnd());
			}

			FileSystem.RemoveFile(TestFileName);

			try
			{
				using (var stream = new StreamReader(FileSystem.OpenRead(TestFileName)))
				{
				}
				Assert.Fail();
			}
			catch (Exception)
			{
			}
		}
	}
}