using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Schema;
using System.IO;

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
                new SchemaComparisionReport());

            Assert.IsNotNull(resultSchema);
            Assert.AreEqual("TestModel", resultSchema.Title, "Expected the title for the base schema");
            SchemaAssertions.AssertSingleProperty(resultSchema, "name", JSchemaType.Integer);
        }

        [TestMethod]
        [Description("The base schema has an invalid type for a property that can't be converted to the one in the schema")]
        public void TestInvalidTypeChange()
        {
            var comparer = new SchemaComparer();
            var report = new SchemaComparisionReport();
            var resultSchema = comparer.Apply(GetSchema(Path.Combine(TestContext.DeploymentDirectory, "Comparisions/invalidTypeChangeInBase.json")),
                GetSchema(Path.Combine(TestContext.DeploymentDirectory, "Comparisions/invalidTypeChangeInBase_base.json")),
                report);

            var failures = report.GetFailures().ToList();
            Assert.AreEqual(1, failures.Count, "one failure expected.");
            Assert.IsInstanceOfType(failures[0], typeof(TypeChangeFailure), "Expected to be a TypeChangeFailure");
            Assert.AreEqual("name", ((TypeChangeFailure)failures[0]).PropertyName);
        }

        [TestMethod]
        [Description("An info schould be available on missing a property in the base schema.")]
        public void TestInfoOnMissingPropertyInBaseSchema()
        {
            var comparer = new SchemaComparer();
            var report = new SchemaComparisionReport();
            var resultSchema = comparer.Apply(GetSchema(Path.Combine(TestContext.DeploymentDirectory, "Comparisions/infoOnMissingPropertyInBaseSchema.json")),
                GetSchema(Path.Combine(TestContext.DeploymentDirectory, "Comparisions/infoOnMissingPropertyInBaseSchema_base.json")),
                report);

            var infos = report.GetInfos().ToList();
            Assert.AreEqual(1, infos.Count, "one failure expected.");
            Assert.IsInstanceOfType(infos[0], typeof(MissingPropertyInfo), "Expected to be a MissingPropertyInfo");
            Assert.AreEqual("missingPropertyInBase", ((MissingPropertyInfo)infos[0]).PropertyName);
        }

        [TestMethod]
        [Description("Any property that is defined in the base schema should be in the result schema.")]
        public void TestAnyPropertyFromBaseSchemaShouldBeTaken()
        {
            var comparer = new SchemaComparer();
            var report = new SchemaComparisionReport();
            var resultSchema = comparer.Apply(GetSchema(Path.Combine(TestContext.DeploymentDirectory, "Comparisions/anyPropertyInMoreSpecific.json")),
                GetSchema(Path.Combine(TestContext.DeploymentDirectory, "Comparisions/anyPropertyInMoreSpecific_base.json")),
                report);

            Assert.IsNotNull(resultSchema);
            Assert.AreEqual("val1", resultSchema.Properties["name"].Format);
            //Assert.AreEqual("TestModel", resultSchema.Properties["prop2"], "val2");
        }

        private static JSchema GetSchema(string path)
        {
            string content;
            using (var reader = new StreamReader(path))
            {
                content = reader.ReadToEnd();
            }

            return JSchema.Parse(content);
        }
    }
}
