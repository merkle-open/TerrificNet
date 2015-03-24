using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
		public async Task TestWrite()
		{
			Assert.AreEqual(false, FileSystem.FileExists(TestFileName));

			var completion = new TaskCompletionSource<IFileInfo>();
			await FileSystem.SubscribeAsync(TestFilePattern, info => completion.SetResult(info)).ConfigureAwait(false);

			using (var stream = new StreamWriter(FileSystem.OpenWrite(TestFileName)))
			{
				stream.Write("123456");
			}

			await completion.Task.ConfigureAwait(false);
			Assert.AreEqual(true, FileSystem.FileExists(TestFileName));

			FileSystem.RemoveFile(TestFileName);
		}

		[TestMethod]
		public async Task TestReWrite()
		{
			var completion = new TaskCompletionSource<IFileInfo>();
			await FileSystem.SubscribeAsync(TestFilePattern, info => completion.SetResult(info)).ConfigureAwait(false);
			
			Assert.AreEqual(false, FileSystem.FileExists(TestFileName));

			using (var stream = new StreamWriter(FileSystem.OpenWrite(TestFileName)))
			{
				stream.Write("123456");
			}

			await completion.Task.ConfigureAwait(false);
			Assert.AreEqual(true, FileSystem.FileExists(TestFileName));
			completion = new TaskCompletionSource<IFileInfo>();

			using (var stream = new StreamReader(FileSystem.OpenRead(TestFileName)))
			{
				Assert.AreEqual("123456", stream.ReadToEnd());
			}

			using (var stream = new StreamWriter(FileSystem.OpenWrite(TestFileName)))
			{
				stream.Write("654321");
			}

			await completion.Task.ConfigureAwait(false);
			Assert.AreEqual(true, FileSystem.FileExists(TestFileName));
			completion = new TaskCompletionSource<IFileInfo>();

			using (var stream = new StreamReader(FileSystem.OpenRead(TestFileName)))
			{
				Assert.AreEqual("654321", stream.ReadToEnd());
			}

			FileSystem.RemoveFile(TestFileName);
			await completion.Task.ConfigureAwait(false);
			
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