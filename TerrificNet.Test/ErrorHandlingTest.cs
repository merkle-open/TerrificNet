using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.Cache;
using TerrificNet.ViewEngine.ViewEngines;
using Veil;
using Veil.Helper;

namespace TerrificNet.Test
{
	[TestClass]
	public class ErrorHandlingTest
	{
		[TestMethod]
		public async Task TestPropertyNotBindable_ThrowsExpection()
		{
		    const string templateId = "views/test";
            const string input = "<p>{{name}}</p>";

		    try
		    {
		        await Execute(input, templateId, new object(), typeof(object));
		    }
		    catch (VeilCompilerException ex)
            {
                Assert.IsNotNull(ex.Message);
                Assert.IsNotNull(ex.Node);
                Assert.IsNotNull(ex.Node.Location);
                Assert.AreEqual(3, ex.Node.Location.Index);
                Assert.AreEqual(8, ex.Node.Location.Length);
                Assert.AreEqual(templateId, ex.Node.Location.TemplateId);
                return;
            }
            Assert.Fail("Expected a VeilCompilerException");
		}

        [TestMethod]
        public async Task TestParseInvalidBlockStatementMissingEndThrowsException()
        {
            const string templateId = "views/test";
            const string input = "<p>{{#if name}}</p>";

            try
            {
                await Execute(input, templateId, new object(), typeof(object));
            }
            catch (VeilParserException ex)
            {
                Assert.IsNotNull(ex.Message);
                Assert.IsNotNull(ex.Location);
                Assert.AreEqual(15, ex.Location.Index);
                Assert.AreEqual(4, ex.Location.Length);
                Assert.AreEqual(templateId, ex.Location.TemplateId);
                return;
            }
            Assert.Fail("Expected a VeilCompilerException");
        }

	    private static async Task<string> Execute(string input, string templateId, object model, Type modelType)
	    {
	        var cacheProvider = new MemoryCacheProvider();
	        var handlerFactory = new Mock<IHelperHandlerFactory>();

	        var namingRule = new NamingRule();
	        var templateInfo = new StringTemplateInfo(templateId, input);

	        var viewEngine = new VeilViewEngine(cacheProvider, handlerFactory.Object, namingRule);

	        var view = await viewEngine.CreateViewAsync(templateInfo, modelType)
	            .ConfigureAwait(false);

	        var builder = new StringBuilder();
	        using (var writer = new StringWriter(builder))
	        {
	            var context = new RenderingContext(writer);
	            await view.RenderAsync(model, context);
	        }
	        return builder.ToString();
	    }
	}
}
