using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TerrificNet.Generator;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.Cache;
using TerrificNet.ViewEngine.SchemaProviders;
using TerrificNet.ViewEngine.ViewEngines;
using Veil;
using Veil.Helper;

namespace TerrificNet.Test
{
	[TestClass]
	public class ErrorHandlingTest
	{
		[TestMethod]
		public async Task TestInvalidSyntax()
		{
			var cacheProvider = new MemoryCacheProvider();
			var handlerFactory = new Mock<IHelperHandlerFactory>();

			var namingRule = new NamingRule();
			const string input = "<p>{{name</p>}}";
			var templateInfo = new StringTemplateInfo("views/test", input);

			var viewEngine = new VeilViewEngine(cacheProvider, handlerFactory.Object, namingRule);

			var view = await viewEngine.CreateViewAsync(templateInfo)
				.ConfigureAwait(false);

			var builder = new StringBuilder();
			using (var writer = new StringWriter(builder))
			{
				var context = new RenderingContext(writer);
				await view.RenderAsync(new object(), context);
			}
		}
	}
}
