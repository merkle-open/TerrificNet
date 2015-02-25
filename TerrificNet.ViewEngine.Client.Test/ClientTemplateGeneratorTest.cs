using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TerrificNet.ViewEngine.Client.Javascript;
using TerrificNet.ViewEngine.ViewEngines;
using Veil.Compiler;
using Veil.Helper;

namespace TerrificNet.ViewEngine.Client.Test
{
	[TestClass]
	public class ClientTemplateGeneratorTest
	{
		[TestMethod]
		public void TestClientTemplateWithoutHandlebars()
		{
			string input = "<html>gugus</html>";
			var generator = CreateClientTemplateGenerator();

			var clientContext = new Mock<IClientContext>();
			clientContext.Setup(c => c.WriteLiteral(input));

			generator.GenerateForTemplate(input, clientContext.Object, new JavascriptClientModel("model"));

			clientContext.VerifyAll();
		}

		[TestMethod]
		public void TestClientTemplateWithExpression()
		{
			string input = "<html>{{test}}</html>";
			var generator = CreateClientTemplateGenerator();

			var clientContext = new Mock<IClientContext>(MockBehavior.Strict);
			clientContext.Setup(c => c.WriteLiteral("<html>"));
            clientContext.Setup(c => c.WriteEncodeExpression(GetModelExpression("model.test")));
			clientContext.Setup(c => c.WriteLiteral("</html>"));

			generator.GenerateForTemplate(input, clientContext.Object, new JavascriptClientModel("model"));

			clientContext.VerifyAll();
		}

		[TestMethod]
		public void TestClientTemplateWithComplexExpression()
		{
			string input = "<html>{{test.prop1}}</html>";
			var generator = CreateClientTemplateGenerator();

			var clientContext = new Mock<IClientContext>(MockBehavior.Strict);
			clientContext.Setup(c => c.WriteLiteral("<html>"));
            clientContext.Setup(c => c.WriteEncodeExpression(GetModelExpression("model.test.prop1")));
			clientContext.Setup(c => c.WriteLiteral("</html>"));

			generator.GenerateForTemplate(input, clientContext.Object, new JavascriptClientModel("model"));

			clientContext.VerifyAll();
		}

		[TestMethod]
		public void TestClientTemplateWithServeralExpression()
		{
			string input = "<html>{{prop1}}{{prop2}}</html>";
			var generator = CreateClientTemplateGenerator();

			var clientContext = new Mock<IClientContext>(MockBehavior.Strict);
			clientContext.Setup(c => c.WriteLiteral("<html>"));
            clientContext.Setup(c => c.WriteEncodeExpression(GetModelExpression("model.prop1")));
            clientContext.Setup(c => c.WriteEncodeExpression(GetModelExpression("model.prop2")));
			clientContext.Setup(c => c.WriteLiteral("</html>"));

			generator.GenerateForTemplate(input, clientContext.Object, new JavascriptClientModel("model"));

			clientContext.VerifyAll();
		}

		[TestMethod]
		public void TestClientTemplateWithBlockExpression()
		{
			string input = "<html>{{anyother}}{{#each test.prop1}}<li>{{name}}</li>{{/each}}{{anyafter}}</html>";
			var generator = CreateClientTemplateGenerator();

			var clientContext = new Mock<IClientContext>(MockBehavior.Strict);
			clientContext.Setup(c => c.WriteLiteral("<html>"));
            clientContext.Setup(c => c.WriteEncodeExpression(GetModelExpression("model.anyother")));
			clientContext.Setup(c => c.BeginIterate(GetModelExpression("model.test.prop1"))).Returns(new JavascriptClientModel("item"));
			clientContext.Setup(c => c.WriteLiteral("<li>"));
            clientContext.Setup(c => c.WriteEncodeExpression(GetModelExpression("item.name")));
			clientContext.Setup(c => c.WriteLiteral("</li>"));
			clientContext.Setup(c => c.EndIterate());
            clientContext.Setup(c => c.WriteEncodeExpression(GetModelExpression("model.anyafter")));
			clientContext.Setup(c => c.WriteLiteral("</html>"));

			generator.GenerateForTemplate(input, clientContext.Object, new JavascriptClientModel("model"));

			clientContext.VerifyAll();
		}

        [TestMethod]
        public void TestClientTemplateWithBlockExpressionThis()
        {
            string input = "<html>{{anyother}}{{#each test.prop1}}<li>{{this}}</li>{{/each}}{{anyafter}}</html>";
            var generator = CreateClientTemplateGenerator();

            var clientContext = new Mock<IClientContext>(MockBehavior.Strict);
            clientContext.Setup(c => c.WriteLiteral("<html>"));
            clientContext.Setup(c => c.WriteEncodeExpression(GetModelExpression("model.anyother")));
            clientContext.Setup(c => c.BeginIterate(GetModelExpression("model.test.prop1"))).Returns(new JavascriptClientModel("item"));
            clientContext.Setup(c => c.WriteLiteral("<li>"));
            clientContext.Setup(c => c.WriteEncodeExpression(GetModelExpression("item")));
            clientContext.Setup(c => c.WriteLiteral("</li>"));
            clientContext.Setup(c => c.EndIterate());
            clientContext.Setup(c => c.WriteEncodeExpression(GetModelExpression("model.anyafter")));
            clientContext.Setup(c => c.WriteLiteral("</html>"));

            generator.GenerateForTemplate(input, clientContext.Object, new JavascriptClientModel("model"));

            clientContext.VerifyAll();
        }

        [TestMethod]
        public void TestClientTemplateWithNestedIterationExpression()
        {
            string input = "<html>{{#each test.prop1}}<li>{{#each values}}{{name}}{{/each}}</li>{{/each}}</html>";
            var generator = CreateClientTemplateGenerator();

            var clientContext = new Mock<IClientContext>(MockBehavior.Strict);
            clientContext.Setup(c => c.WriteLiteral("<html>"));
            clientContext.Setup(c => c.BeginIterate(GetModelExpression("model.test.prop1"))).Returns(new JavascriptClientModel("item"));
            clientContext.Setup(c => c.WriteLiteral("<li>"));
            clientContext.Setup(c => c.BeginIterate(GetModelExpression("item.values"))).Returns(new JavascriptClientModel("item2"));
            clientContext.Setup(c => c.WriteEncodeExpression(GetModelExpression("item2.name")));
            clientContext.Setup(c => c.EndIterate());
            clientContext.Setup(c => c.WriteLiteral("</li>"));
            clientContext.Setup(c => c.EndIterate());
            clientContext.Setup(c => c.WriteLiteral("</html>"));

            generator.GenerateForTemplate(input, clientContext.Object, new JavascriptClientModel("model"));

            clientContext.VerifyAll();
        }


		[TestMethod]
		public void TestClientTemplateWithConditionalExpression()
		{
			string input = "<html>{{#if test.prop1}}output{{/if}}</html>";
			var generator = CreateClientTemplateGenerator();

			var clientContext = new Mock<IClientContext>(MockBehavior.Strict);
			clientContext.Setup(c => c.WriteLiteral("<html>"));
			clientContext.Setup(c => c.BeginIf(GetModelExpression("model.test.prop1")));
			clientContext.Setup(c => c.WriteLiteral("output"));
			clientContext.Setup(c => c.EndIf());
			clientContext.Setup(c => c.WriteLiteral("</html>"));

			generator.GenerateForTemplate(input, clientContext.Object, new JavascriptClientModel("model"));

			clientContext.VerifyAll();
		}

        [TestMethod]
        public void TestClientTemplateWithUnencodeExpression()
        {
            string input = "<html>{{{expression}}}</html>";
            var generator = CreateClientTemplateGenerator();

            var clientContext = new Mock<IClientContext>(MockBehavior.Strict);
            clientContext.Setup(c => c.WriteLiteral("<html>"));
            clientContext.Setup(c => c.WriteExpression(GetModelExpression("model.expression")));
            clientContext.Setup(c => c.WriteLiteral("</html>"));

            generator.GenerateForTemplate(input, clientContext.Object, new JavascriptClientModel("model"));

            clientContext.VerifyAll();
        }

		private static ClientTemplateGenerator CreateClientTemplateGenerator()
		{
			IMemberLocator memberLocator = new MemberLocatorFromNamingRule(new NamingRule());
			var helperHandlersMock = new Mock<IHelperHandlerFactory>();
			helperHandlersMock.Setup(f => f.Create()).Returns(Enumerable.Empty<IHelperHandler>());

			var generator = new ClientTemplateGenerator(helperHandlersMock.Object, memberLocator);
			return generator;
		}

		private static JavascriptClientModel GetModelExpression(string expression)
		{
			return It.Is<JavascriptClientModel>(m => m.ToString() == expression);
		}

	}
}
