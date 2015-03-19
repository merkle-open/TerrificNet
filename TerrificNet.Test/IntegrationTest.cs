using System;
using System.IO;
using System.Linq;
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
    public class IntegrationTest
    {
        [TestMethod]
        public async Task TemplateEngineShouldUseSameNamingConventionForBinding()
        {
            var cacheProvider = new MemoryCacheProvider();
            var handlerFactory = new Mock<IHelperHandlerFactory>();

            handlerFactory.Setup(f => f.Create()).Returns(Enumerable.Empty<IHelperHandler>());

            var namingRule = new NamingRule();
            var schemaProvider = new HandlebarsViewSchemaProvider(null, new MemberLocatorFromNamingRule(namingRule));
            var codeGenerator = new JsonSchemaCodeGenerator(namingRule);
            const string input = "<p>{{name}}</p><p>{{first_name}}</p>";
            var templateInfo = new StringTemplateInfo("views/test", input);

            var schema = await schemaProvider.GetSchemaFromTemplateAsync(templateInfo);            
            var modelType = codeGenerator.Compile(schema);

            var viewEngine = new VeilViewEngine(cacheProvider, handlerFactory.Object,  namingRule);

            var view = await viewEngine.CreateViewAsync(templateInfo, modelType).ConfigureAwait(false);

            var model = Activator.CreateInstance(modelType);

            modelType.GetProperty("Name").SetValue(model, "{{name}}");
            modelType.GetProperty("FirstName").SetValue(model, "{{first_name}}");

            var writer = new StringWriter();
            await view.RenderAsync(model, new RenderingContext(writer));
            var stringResult = writer.ToString();

            Assert.AreEqual(input, stringResult);
        }

    }
}
