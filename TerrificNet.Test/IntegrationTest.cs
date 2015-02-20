using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TerrificNet.Generator;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.Cache;
using TerrificNet.ViewEngine.SchemaProviders;
using TerrificNet.ViewEngine.ViewEngines;
using TerrificNet.ViewEngine.ViewEngines.TemplateHandler;

namespace TerrificNet.Test
{
    [TestClass]
    public class IntegrationTest
    {
        [TestMethod]
        public void TemplateEngineShouldUseSameNamingConventionForBinding()
        {
            var cacheProvider = new MemoryCacheProvider();
            var handlerFactory = new Mock<IRenderingHelperHandlerFactory>();

            handlerFactory.Setup(f => f.Create()).Returns(Enumerable.Empty<IRenderingHelperHandler>());

            var namingRule = new NamingRule();
            var schemaProvider = new HandlebarsViewSchemaProvider(null, new MemberLocatorFromNamingRule(namingRule));
            var codeGenerator = new JsonSchemaCodeGenerator(namingRule);
            const string input = "<p>{{name}}</p><p>{{first_name}}</p>";
            var templateInfo = new StringTemplateInfo("views/test", input);

            var schema = schemaProvider.GetSchemaFromTemplate(templateInfo);            
            var modelType = codeGenerator.Compile(schema);

            var viewEngine = new VeilViewEngine(cacheProvider, handlerFactory.Object,  namingRule);

            IView view;
            viewEngine.TryCreateView(templateInfo, modelType, out view);

            var model = Activator.CreateInstance(modelType);

            modelType.GetProperty("Name").SetValue(model, "{{name}}");
            modelType.GetProperty("FirstName").SetValue(model, "{{first_name}}");

            var writer = new StringWriter();
            view.Render(model, new RenderingContext(writer));
            var stringResult = writer.ToString();

            Assert.AreEqual(input, stringResult);
        }

    }
}
