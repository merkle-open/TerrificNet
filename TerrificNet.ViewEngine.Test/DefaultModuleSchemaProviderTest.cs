using System.Collections.Generic;
using System.Threading.Tasks;
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
		public async Task TestUseSchemaFromDefaultTemplateIfNoSkins()
		{
			var templateInfo = new StringTemplateInfo("test", "");
            var schema = new JSchema();

			var templateSchemaProvider = new Mock<ISchemaProvider>();
			templateSchemaProvider.Setup(f => f.GetSchemaFromTemplateAsync(templateInfo)).Returns(Task.FromResult(schema));

			var moduleDefintion = new ModuleDefinition("testmod", templateInfo, null);

			var underTest = new DefaultModuleSchemaProvider(templateSchemaProvider.Object);

			var result = await underTest.GetSchemaFromModuleAsync(moduleDefintion);
			Assert.AreEqual(schema, result);
		}

		[TestMethod]
		public void TestUseCombinedSkinsSchemaWhenNoDefaultTemplate()
		{
			var templateInfo = new StringTemplateInfo("test", "");
			var templateInfo2 = new StringTemplateInfo("test2", "");
			var schema1 = new JSchema();
            var schema2 = new JSchema();

			var templateSchemaProvider = new Mock<ISchemaProvider>();
			templateSchemaProvider.Setup(f => f.GetSchemaFromTemplateAsync(templateInfo)).Returns(Task.FromResult(schema1));
			templateSchemaProvider.Setup(f => f.GetSchemaFromTemplateAsync(templateInfo2)).Returns(Task.FromResult(schema2));

			var combiner = new Mock<SchemaCombiner>();
			combiner.Setup(c =>
                c.Apply(It.Is<JSchema>(s => s == schema1), It.Is<JSchema>(s => s == schema2), It.IsAny<SchemaComparisionReport>(), It.IsAny<string>()))
                .Returns(new JSchema());

			var moduleDefintion = new ModuleDefinition("testmod", null, new Dictionary<string, TemplateInfo>
            {
				{"skin1", templateInfo},
                {"skin2", templateInfo2}
            });

			var underTest = new DefaultModuleSchemaProvider(combiner.Object, templateSchemaProvider.Object);

			var result = underTest.GetSchemaFromModuleAsync(moduleDefintion);

			Assert.IsNotNull(result);

			combiner.Verify();
		}
	}
}
