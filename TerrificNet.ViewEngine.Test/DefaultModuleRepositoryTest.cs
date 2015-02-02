using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TerrificNet.Test;
using TerrificNet.ViewEngine.Config;

namespace TerrificNet.ViewEngine.Test
{
    [TestClass]
    public class DefaultModuleRepositoryTest
    {
        [TestMethod]
        public void TestOnlyUseTemplatesFromModulePath()
        {
            var templateRepository = CreateRepository("modules/Mod1/Mod1", "modules/Mod2/Mod2", "layouts/Layout1");
            var terrificNetConfig = CreateConfig();

            var underTest = new DefaultModuleRepository(terrificNetConfig, templateRepository);
            var result = underTest.GetAll().ToList();

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void TestModuleContainsSkins()
        {
            var templateRepository = CreateRepository("modules/Mod1/Mod1-skin1", "modules/Mod1/Mod1-skin2");
            var terrificNetConfig = CreateConfig();

            var underTest = new DefaultModuleRepository(terrificNetConfig, templateRepository);
            var result = underTest.GetAll().ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(2, result[0].Skins.Count, "Two skins expected for module");
            Assert.IsTrue(result[0].Skins.ContainsKey("skin1"), "Expected to have skin with name skin1");
            Assert.IsTrue(result[0].Skins.ContainsKey("skin2"), "Expected to have skin with name skin2");
        }


        private static ITerrificNetConfig CreateConfig()
        {
            var config = new Mock<ITerrificNetConfig>();
            config.Setup(c => c.ModulePath).Returns("modules");

            return config.Object;
        }

        private static ITemplateRepository CreateRepository(params string[] templates)
        {
            var templateRepo = new Mock<ITemplateRepository>();
            templateRepo.Setup(t => t.GetAll()).Returns(templates.Select(id => new IntegrationTest.StringTemplateInfo(id, "")));
            return templateRepo.Object;
        }
    }
}
