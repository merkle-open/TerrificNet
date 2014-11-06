using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Schema;
using Nustache.Core;
using Veil.Handlebars;

namespace TerrificNet.ViewEngine.Schema.Test
{
    [TestClass]
    public class SchemaExtractorTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [Description("Test whether a single property is included in the schema")]
        public void TestSimpleSingleProperty()
        {
            var schemaExtractor = new SchemaExtractor(new HandlebarsTemplateParserRegistration());
            var schema = schemaExtractor.Run(Path.Combine(TestContext.DeploymentDirectory, "simpleSingleProperty.mustache"));

            SchemaAssertions.AssertSingleProperty(schema, "Name", JsonSchemaType.String);
        }

        [TestMethod]
        [Description("Test whether a single property is included in the schema")]
        public void TestSimpleSinglePropertyPath()
        {
            var schemaExtractor = new SchemaExtractor(new HandlebarsTemplateParserRegistration());
            var schema = schemaExtractor.Run(Path.Combine(TestContext.DeploymentDirectory, "simpleSinglePropertyPath.mustache"));

            SchemaAssertions.AssertSingleProperty(schema, "Customer", JsonSchemaType.Object);
            SchemaAssertions.AssertSingleProperty(schema.Properties["Customer"], "Name", JsonSchemaType.String);
        }

        [TestMethod]
        [Description("Test whether a multiple properties are included in the schema")]
        public void TestMultipleProperties()
        {
            var schemaExtractor = new SchemaExtractor(new HandlebarsTemplateParserRegistration());
            var schema = schemaExtractor.Run(Path.Combine(TestContext.DeploymentDirectory, "multipleProperties.mustache"));

            SchemaAssertions.AssertSingleProperty(schema, "Title", JsonSchemaType.String);
            SchemaAssertions.AssertSingleProperty(schema, "Customer", JsonSchemaType.Object);
            SchemaAssertions.AssertSingleProperty(schema.Properties["Customer"], "Name", JsonSchemaType.String);
            SchemaAssertions.AssertSingleProperty(schema.Properties["Customer"], "Age", JsonSchemaType.String);
            SchemaAssertions.AssertSingleProperty(schema.Properties["Customer"], "Order", JsonSchemaType.Object);
            SchemaAssertions.AssertSingleProperty(schema.Properties["Customer"].Properties["Order"], "Count", JsonSchemaType.String);
        }

        [TestMethod]
        [Description("A property inside a if expression is not required")]
        public void TestNoRequiredPropertys()
        {
            var schemaExtractor = new SchemaExtractor(new HandlebarsTemplateParserRegistration());
            var schema = schemaExtractor.Run(Path.Combine(TestContext.DeploymentDirectory, "noRequiredProperty.mustache"));

            SchemaAssertions.AssertSingleProperty(schema, "Customer", JsonSchemaType.Object);
            SchemaAssertions.AssertSingleProperty(schema.Properties["Customer"], "Name", JsonSchemaType.String, false);
        }

        [TestMethod]
        [Description("A property only inside a if expression is a boolean")]
        public void TestBooleanProperty()
        {
            var schemaExtractor = new SchemaExtractor(new HandlebarsTemplateParserRegistration());
            var schema = schemaExtractor.Run(Path.Combine(TestContext.DeploymentDirectory, "booleanProperty.mustache"));

            SchemaAssertions.AssertSingleProperty(schema, "Customer", JsonSchemaType.Object);
            SchemaAssertions.AssertSingleProperty(schema.Properties["Customer"], "Name", JsonSchemaType.String);
            SchemaAssertions.AssertSingleProperty(schema.Properties["Customer"], "HasName", JsonSchemaType.Boolean, false);
        }

        [TestMethod]
        [Description("The helper shoudn't be part of the schema.")]
        public void TestIgnoreHelpers()
        {
            Helpers.Register("helper", (context, arguments, options, fn, inverse) => { });

            var schemaExtractor = new SchemaExtractor(new HandlebarsTemplateParserRegistration());
            var schema = schemaExtractor.Run(Path.Combine(TestContext.DeploymentDirectory, "ignoreHelpers.mustache"));

            SchemaAssertions.AssertSingleProperty(schema, "Customer", JsonSchemaType.Object);
            SchemaAssertions.AssertSingleProperty(schema.Properties["Customer"], "Name", JsonSchemaType.String);

            Assert.IsFalse(schema.Properties.ContainsKey("helper param=\"val1\""), "No property helper should be inside the schema.");
            Assert.IsTrue(schema.Properties.ContainsKey("noregistredHelper param=\"val1\""), "The none registred helpers should be still included.");
            SchemaAssertions.AssertSingleProperty(schema, "variableExpressionWithWhitespace", JsonSchemaType.String);
        }

        [TestMethod]
        [Description("A property used inside a each expression is a array")]
        public void TestArrayProperty()
        {
            var schemaExtractor = new SchemaExtractor(new HandlebarsTemplateParserRegistration());
            var schema = schemaExtractor.Run(Path.Combine(TestContext.DeploymentDirectory, "arrayProperty.mustache"));

            SchemaAssertions.AssertSingleProperty(schema, "Customer", JsonSchemaType.Object);
            SchemaAssertions.AssertSingleProperty(schema.Properties["Customer"], "Addresses", JsonSchemaType.Array);
            Assert.IsNotNull(schema.Properties["Customer"].Properties["Addresses"].Items, "an items array should be given for an array type.");
            Assert.AreEqual(1, schema.Properties["Customer"].Properties["Addresses"].Items.Count, "expectects exactly on item inside items");

            SchemaAssertions.AssertSingleProperty(schema.Properties["Customer"].Properties["Addresses"].Items[0], "Street", JsonSchemaType.String);
            SchemaAssertions.AssertSingleProperty(schema.Properties["Customer"].Properties["Addresses"].Items[0], "ZipCode", JsonSchemaType.String);
        }



    }

}
