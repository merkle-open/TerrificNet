using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TerrificNet.Generator;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.Cache;
using TerrificNet.ViewEngine.SchemaProviders;
using TerrificNet.ViewEngine.ViewEngines;

namespace TerrificNet.Test
{
    [TestClass]
    public class IntegrationTest
    {
        [TestMethod]
        public void TemplateEngineShouldUseSameNamingConventionForBinding()
        {
            var cacheProvider = new MemoryCacheProvider();
            var handlerFactory = new Mock<ITerrificTemplateHandlerFactory>();
            var handler = new Mock<ITerrificTemplateHandler>();

            handlerFactory.Setup(f => f.Create()).Returns(handler.Object);

            var schemaProvider = new HandlebarsViewSchemaProvider();
            var namingRule = new NamingRule();
            var codeGenerator = new JsonSchemaCodeGenerator(namingRule);
            var templateInfo = new StringTemplateInfo("test", "<p>{{name}}</p><p>{{first_name}}</p>");

            var schema = schemaProvider.GetSchemaFromTemplate(templateInfo);
            // TODO: Remove
            schema.Title = templateInfo.Id + "Model";
            
            var modelType = codeGenerator.Compile(schema);

            var viewEngine = new VeilViewEngine(cacheProvider, handlerFactory.Object, namingRule);

            IView view;
            viewEngine.TryCreateView(templateInfo, modelType, out view);

            var model = Activator.CreateInstance(modelType);

            var writer = new StringWriter();
            view.Render(model, new RenderingContext(writer));

            var stringResult = writer.ToString();
        }

        public class StringTemplateInfo : TemplateInfo
        {
            private readonly string _content;

            public StringTemplateInfo(string id, string content) : base(id)
            {
                _content = content;
            }

            public override Stream Open()
            {
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                writer.Write(_content);
                writer.Flush();
                stream.Position = 0;
                return stream;
            }
        }
    }
}
