using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

namespace TerrificNet.ViewEngine.Schema.Test
{
    [TestClass]
    public class SchemaComparerTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [Description("The more specific property attributes should be taken from the base set")]
        public void TestMoreSpecific()
        {
            var comparer = new SchemaComparer();
            var resultSchema = comparer.Apply(GetSchema(Path.Combine(TestContext.DeploymentDirectory, "Comparisions/moreSpecific.json")), 
                GetSchema(Path.Combine(TestContext.DeploymentDirectory, "Comparisions/moreSpecific_base.json")),
                new List<SchemaComparisonFailure>());

            Assert.IsNotNull(resultSchema);
            Assert.AreEqual("TestModel", resultSchema.Title, "Expected the title for the base schema");
            SchemaAssertions.AssertSingleProperty(resultSchema, "name", JsonSchemaType.Integer);
        }

        [TestMethod]
        [Description("The base schema has an invalid type for a property that can't be converted to the one in the schema")]
        public void TestInvalidTypeChange()
        {
            var comparer = new SchemaComparer();
            var failures = new List<SchemaComparisonFailure>();
            var resultSchema = comparer.Apply(GetSchema(Path.Combine(TestContext.DeploymentDirectory, "Comparisions/invalidTypeChangeInBase.json")),
                GetSchema(Path.Combine(TestContext.DeploymentDirectory, "Comparisions/invalidTypeChangeInBase_base.json")),
                failures);

            Assert.AreEqual(1, failures.Count, "one failure expected.");
            Assert.IsInstanceOfType(failures[0], typeof(TypeChangeFailure), "Expected to be a TypeChangeFailure");
            Assert.AreEqual("name", ((TypeChangeFailure)failures[0]).PropertyName);
        }

        private static JsonSchema GetSchema(string path)
        {
            string content = null;
            using (var reader = new StreamReader(path))
            {
                content = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<JsonSchema>(content);
        }
    }
}
