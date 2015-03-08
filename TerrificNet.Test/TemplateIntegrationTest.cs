using System.Collections.Generic;
using System.Data;
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
using Veil;
using Veil.Helper;

namespace TerrificNet.Test
{
    [TestClass]
    public class TemplateIntegrationTest
    {
        private string _testName;
        private string _dataFile;
        private string _templateFile;
        private string _resultFile;
        private string _result;

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void TestSetup()
        {
            _dataFile = Path.Combine(this.TestContext.DeploymentDirectory, (string)TestContext.DataRow["data"]);
            _templateFile = Path.Combine(this.TestContext.DeploymentDirectory, (string)TestContext.DataRow["template"]);
            _resultFile = Path.Combine(this.TestContext.DeploymentDirectory, (string)TestContext.DataRow["result"]);
            _testName = Path.GetFileName(Path.GetDirectoryName(_resultFile));

            _result = GetFileContent(_resultFile);
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\test_cases.csv", "test_cases#csv", DataAccessMethod.Sequential)]
        public void TestServerSideRendering()
        {
            var resultString = ExecuteServerSide(_testName, _templateFile, _dataFile);
            Assert.AreEqual(_result, resultString);
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\test_cases.csv", "test_cases#csv", DataAccessMethod.Sequential)]
        public void TestServerSideRenderingWithStronglyTypedModel()
        {
            var resultString = ExecuteServerSideStrongModel(_testName, _templateFile, _dataFile);
            Assert.AreEqual(_result, resultString);
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\test_cases.csv", "test_cases#csv", DataAccessMethod.Sequential)]
        public void TestClientSideRenderingWithJavascript()
        {
            var resultString = ExecuteClientSide(_testName, _templateFile, _dataFile);
            Assert.AreEqual(_result, resultString);
        }

        private static string GetFileContent(string resultFile)
        {
            string result;
            using (var reader = new StreamReader(resultFile))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        private string ExecuteClientSide(string testName, string templateFile, string dataFile)
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

            this.TestContext.BeginTimer("JS Rendering");
            var result = JavascriptClientTest.ExecuteJavascript(view, model, testName);
            this.TestContext.EndTimer("JS Rendering");

            return result;
        }

        private string ExecuteServerSideStrongModel(string testName, string templateFile, string dataFile)
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

            this.TestContext.BeginTimer("ServerStrong");
            var builder = new StringBuilder();
            using (var writer = new StringWriter(builder))
            {
                view.Render(model, new RenderingContext(writer));
            }
            var resultString = builder.ToString();
            this.TestContext.EndTimer("ServerStrong");
            return resultString;
        }

        private string ExecuteServerSide(string testName, string templateFile, string dataFile)
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
            using (var reader = new StreamReader(dataFile))
            {
                model = new JavaScriptSerializer().Deserialize(reader.ReadToEnd(), typeof(Dictionary<string, object>));
            }

            this.TestContext.BeginTimer("Server");
            var builder = new StringBuilder();
            using (var writer = new StringWriter(builder))
            {
                view.Render(model, new RenderingContext(writer));
            }
            var resultString = builder.ToString();
            this.TestContext.EndTimer("Server");
            return resultString;
        }

        private class NullRenderingHelperHandlerFactory : IHelperHandlerFactory
        {
            public IEnumerable<IHelperHandler> Create()
            {
                return Enumerable.Empty<IHelperHandler>();
            }
        }
    }
}
