using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerrificNet.ViewEngine.IO;

namespace TerrificNet.ViewEngine.Test
{
	[TestClass]
	public class FileSystemSubscriptionTest
	{
		protected const string TestFileName = "test.txt";
		protected const string TestFilePattern = "*.txt";

		public TestContext TestContext { get; set; }

		[TestMethod]
		public async Task TestSubscription()
		{
			var fileSystem = new FileSystem(TestContext.TestRunDirectory, true);
			fileSystem.RemoveFile(TestFileName);

			Assert.AreEqual(false, fileSystem.FileExists(TestFileName));

			var c = new TaskCompletionSource<string>();
			var subscription = await fileSystem.SubscribeAsync(TestFilePattern).ConfigureAwait(false);
			subscription.Register(s => c.TrySetResult(s));

			using (var writer = new StreamWriter(fileSystem.OpenWrite(TestFileName)))
			{
				writer.BaseStream.SetLength(0);
				writer.Write("123456789");
			}

			var result = await c.Task.ConfigureAwait(false);
			Assert.AreEqual(TestFileName, new FileInfo(result).Name);
		}
	}
}