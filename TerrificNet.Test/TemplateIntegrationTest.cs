﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using TerrificNet.Generator;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.Cache;
using TerrificNet.ViewEngine.Client;
using TerrificNet.ViewEngine.Client.Javascript;
using TerrificNet.ViewEngine.Client.Test;
using TerrificNet.ViewEngine.IO;
using TerrificNet.ViewEngine.SchemaProviders;
using TerrificNet.ViewEngine.ViewEngines;
using Veil.Helper;

namespace TerrificNet.Test
{
    [TestClass]
    public class TemplateIntegrationTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestBasicTemplates()
        {
            foreach (var testCase in Directory.GetDirectories(Path.Combine(this.TestContext.DeploymentDirectory, "Basic")))
            {
                var dataFile = Path.Combine(testCase, "data.json");
                var templateFile = Path.Combine(testCase, "template.html");
                var resultFile = Path.Combine(testCase, "result.html");
                var testName = Path.GetFileName(testCase);

                string result;
                using (var reader = new StreamReader(resultFile))
                {
                    result = reader.ReadToEnd();
                }

                var resultString = ExecuteServerSide(testName, templateFile, dataFile);
                Assert.AreEqual(result, resultString);

                resultString = ExecuteServerSideStrongModel(testName, templateFile, dataFile);
                Assert.AreEqual(result, resultString);

                resultString = ExecuteClientSide(testName, templateFile, dataFile);
                Assert.AreEqual(result, resultString);
            }
        }

        private static string ExecuteClientSide(string testName, string templateFile, string dataFile)
        {
            var namingRule = new NamingRule();
            var handlerFactory = new NullRenderingHelperHandlerFactory();

            var clientGenerator = new ClientTemplateGenerator(handlerFactory, new MemberLocatorFromNamingRule(namingRule));
            var generator = new JavascriptClientTemplateGenerator("repo", clientGenerator);

            var templateInfo = new FileTemplateInfo(testName, templateFile, new FileSystem());
            var view = generator.Generate(templateInfo);

            object model;
            using (var reader = new StreamReader(dataFile))
            {
                model = new JavaScriptSerializer().Deserialize(reader.ReadToEnd(), typeof(Dictionary<string, object>));
            }

            return JavascriptClientTest.ExecuteJavascript(view, model, testName);
        }

        private static string ExecuteServerSideStrongModel(string testName, string templateFile, string dataFile)
        {
            var cacheProvider = new NullCacheProvider();
            var namingRule = new NamingRule();
            var handlerFactory = new NullRenderingHelperHandlerFactory();

            var templateInfo = new FileTemplateInfo(testName, templateFile, new FileSystem());

            var schemaProvider = new HandlebarsViewSchemaProvider(handlerFactory, new MemberLocatorFromNamingRule(namingRule));

            var generator = new JsonSchemaCodeGenerator(namingRule);
            var schema = schemaProvider.GetSchemaFromTemplate(templateInfo);
            schema.Title = "Model";
            var modelType = generator.Compile(schema);

            var viewEngine = new VeilViewEngine(cacheProvider, handlerFactory, namingRule);
            IView view;
            if (!viewEngine.TryCreateView(templateInfo, modelType, out view))
                Assert.Fail("Could not create view from file '{0}'.", templateFile);

            object model;
            using (var reader = new JsonTextReader(new StreamReader(dataFile)))
            {
                model = JsonSerializer.Create().Deserialize(reader, modelType);
            }

            var builder = new StringBuilder();
            using (var writer = new StringWriter(builder))
            {
                view.Render(model, new RenderingContext(writer));
            }
            var resultString = builder.ToString();
            return resultString;
        }

        private static string ExecuteServerSide(string testName, string templateFile, string dataFile)
        {
            var cacheProvider = new NullCacheProvider();
            var namingRule = new NamingRule();
            var handlerFactory = new NullRenderingHelperHandlerFactory();

            var templateInfo = new FileTemplateInfo(testName, templateFile, new FileSystem());

            var viewEngine = new VeilViewEngine(cacheProvider, handlerFactory, namingRule);
            IView view;
            if (!viewEngine.TryCreateView(templateInfo, out view))
                Assert.Fail("Could not create view from file'{0}'.", templateFile);

            object model;
            using (var reader = new JsonTextReader(new StreamReader(dataFile)))
            {
                model = JsonSerializer.Create().Deserialize(reader);
            }

            var builder = new StringBuilder();
            using (var writer = new StringWriter(builder))
            {
                view.Render(model, new RenderingContext(writer));
            }
            var resultString = builder.ToString();
            return resultString;
        }

        private class NullRenderingHelperHandlerFactory : IRenderingHelperHandlerFactory, IHelperHandlerFactory
        {
            public IEnumerable<IRenderingHelperHandler> Create()
            {
                return Enumerable.Empty<IRenderingHelperHandler>();
            }

            IEnumerable<IHelperHandler> IHelperHandlerFactory.Create()
            {
                return Create();
            }
        }
    }
}