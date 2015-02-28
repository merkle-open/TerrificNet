using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Noesis.Javascript;
using TerrificNet.ViewEngine.Client.Javascript;
using TerrificNet.ViewEngine.IO;
using TerrificNet.ViewEngine.ViewEngines;
using Veil.Compiler;
using Veil.Helper;

namespace TerrificNet.ViewEngine.Client.Test
{
    [TestClass]
    public class JavascriptClientTest
    {
        [TestMethod]
        public void TestJavascriptSimpleModel()
        {
            var generator = new JavascriptClientTemplateGenerator("repo", CreateClientTemplateGenerator());

            var view = generator.Generate(new StringTemplateInfo("test", "hallo {{value}}"));

            Assert.IsNotNull(view);

            var model = new Dictionary<string, object>
                {
                    { "value", "test" }
                };

            var result = ExecuteJavascript(view, model, "test");
            Assert.AreEqual("hallo test", result);
        }

        public static string ExecuteJavascript(string view, object model, string viewName)
        {
            using (var context = new JavascriptContext())
            {
                var fileSystem = new EmbeddedResourceFileSystem(typeof (WebInitializer).Assembly);

                using (var reader = new StreamReader(fileSystem.OpenRead("Core/js/ViewEngine.js")))
                {
                    context.Run(reader.ReadToEnd());
                }

                context.Run("var repo = { actions: {}, register: function(name, action) { this.actions[name] = action; } };");
                context.Run(view);

                context.SetParameter("m", model);
                context.Run(
                    "var ctx = new Tcn.StringRenderingContext(); repo.actions[\""+ viewName +"\"].render(ctx, m); var result = ctx.out;");

                return (string) context.GetParameter("result");
            }
        }

        private static ClientTemplateGenerator CreateClientTemplateGenerator()
        {
            IMemberLocator memberLocator = new MemberLocatorFromNamingRule(new NamingRule());
            var helperHandlersMock = new Mock<IHelperHandlerFactory>();
            helperHandlersMock.Setup(f => f.Create()).Returns(Enumerable.Empty<IHelperHandler>());

            var generator = new ClientTemplateGenerator(helperHandlersMock.Object, memberLocator);
            return generator;
        }
    }
}
