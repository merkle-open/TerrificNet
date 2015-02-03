using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Schema;
using TerrificNet.Test;
using TerrificNet.ViewEngine.Schema;
using TerrificNet.ViewEngine.SchemaProviders;

namespace TerrificNet.ViewEngine.Test
{
	[TestClass]
	public class DefaultModuleSchemaProviderTest
	{
		[TestMethod]
		public void TestUseSchemaFromDefaultTemplateIfNoSkins()
		{
			var templateInfo = new StringTemplateInfo("test", "");
			var schema = new JsonSchema();

			var templateSchemaProvider = new Mock<ISchemaProvider>();
			templateSchemaProvider.Setup(f => f.GetSchemaFromTemplate(templateInfo)).Returns(schema);

			var moduleDefintion = new ModuleDefinition("testmod", templateInfo, null);

			var underTest = new DefaultModuleSchemaProvider(templateSchemaProvider.Object);

			var result = underTest.GetSchemaFromModule(moduleDefintion);
			Assert.AreEqual(schema, result);
		}

		[TestMethod]
		public void TestUseCombinedSkinsSchemaWhenNoDefaultTemplate()
		{
			var templateInfo = new StringTemplateInfo("test", "");
			var templateInfo2 = new StringTemplateInfo("test2", "");
			var schema1 = new JsonSchema();
			var schema2 = new JsonSchema();

			var templateSchemaProvider = new Mock<ISchemaProvider>();
			templateSchemaProvider.Setup(f => f.GetSchemaFromTemplate(templateInfo)).Returns(schema1);
			templateSchemaProvider.Setup(f => f.GetSchemaFromTemplate(templateInfo2)).Returns(schema2);

			var combiner = new Mock<SchemaCombiner>();
			combiner.Setup(c =>
				c.Apply(It.Is<JsonSchema>(s => s == schema1), It.Is<JsonSchema>(s => s == schema2), It.IsAny<SchemaComparisionReport>(), It.IsAny<string>()))
				.Returns(new JsonSchema());

			var moduleDefintion = new ModuleDefinition("testmod", null, new Dictionary<string, TemplateInfo>
            {
				{"skin1", templateInfo},
                {"skin2", templateInfo2}
            });

			var underTest = new DefaultModuleSchemaProvider(combiner.Object, templateSchemaProvider.Object);

			var result = underTest.GetSchemaFromModule(moduleDefintion);

			Assert.IsNotNull(result);

			combiner.Verify();
		}
	}
}
