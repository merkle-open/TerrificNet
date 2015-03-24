using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerrificNet.ViewEngine.IO;

namespace TerrificNet.ViewEngine.Test
{
	[TestClass]
	public class FileSystemSubscriptionTest
	{
		protected PathInfo TestFileName = PathInfo.Create("test.txt");
		protected const string TestFilePattern = "*.txt";

		public TestContext TestContext { get; set; }

		[TestMethod]
		public async Task TestSubscription()
		{
			var fileSystem = new FileSystem(TestContext.TestRunDirectory);
			fileSystem.RemoveFile(TestFileName);

			Assert.AreEqual(false, fileSystem.FileExists(TestFileName));

			var c = new TaskCompletionSource<IFileInfo>();
			using (await fileSystem.SubscribeAsync(TestFilePattern, s => c.TrySetResult(s)).ConfigureAwait(false))
			{
				using (var writer = new StreamWriter(fileSystem.OpenWrite(TestFileName)))
				{
					writer.BaseStream.SetLength(0);
					writer.Write("123456789");
				}

				var result = await c.Task.ConfigureAwait(false);
				Assert.AreEqual(TestFileName.ToString(), Path.GetFileName(result.FilePath.ToString()));
			}
		}

		[TestMethod]
		public async Task TestDirectorySubscription()
		{
			IFileSystem fileSystem = new FileSystem(TestContext.TestRunDirectory);
			fileSystem.RemoveFile(TestFileName);

			Assert.AreEqual(false, fileSystem.FileExists(TestFileName));

			var c = new TaskCompletionSource<IEnumerable<IFileInfo>>();
			using (await fileSystem.SubscribeDirectoryGetFilesAsync(PathInfo.Create(""), "txt", infos =>
				{
					c.SetResult(infos);
				}).ConfigureAwait(false))
			{
				using (var writer = new StreamWriter(fileSystem.OpenWrite(TestFileName)))
				{
					writer.BaseStream.SetLength(0);
					writer.Write("123456789");
				}

				var result = await c.Task.ConfigureAwait(false);
				CollectionAssert.AreEquivalent(new[] { TestFileName }, result.Select(i => i.FilePath).ToList());
			}
		}
	}
}