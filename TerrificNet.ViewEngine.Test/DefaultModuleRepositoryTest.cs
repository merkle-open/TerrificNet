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
            Assert.AreEqual("modules/Mod1", result[0].Id);
            Assert.AreEqual("modules/Mod2", result[1].Id);
        }

        [TestMethod]
        public void TestModuleContainsSkins()
        {
            var templateRepository = CreateRepository("modules/Mod1/Mod1-skin1", "modules/Mod1/Mod1-skin2");
            var terrificNetConfig = CreateConfig();

            var underTest = new DefaultModuleRepository(terrificNetConfig, templateRepository);
            var result = underTest.GetAll().ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("modules/Mod1", result[0].Id, "Module name should be modules/Mod1");
            Assert.AreEqual(2, result[0].Skins.Count, "Two skins expected for module");
            Assert.IsTrue(result[0].Skins.ContainsKey("skin1"), "Expected to have skin with name skin1");
            Assert.IsTrue(result[0].Skins.ContainsKey("skin2"), "Expected to have skin with name skin2");
        }

        [TestMethod]
        public void TestModuleContainsUseDefaultTemplateWithSameName()
        {
            var templateRepository = CreateRepository("modules/Mod1/Mod1", "modules/Mod1/Mod1-skin1", "modules/Mod1/Mod1-skin2");
            var terrificNetConfig = CreateConfig();

            var underTest = new DefaultModuleRepository(terrificNetConfig, templateRepository);
            var result = underTest.GetAll().ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("modules/Mod1", result[0].Id, "Module name should be modules/Mod1");
            Assert.IsNotNull(result[0].DefaultTemplate);
            Assert.AreEqual("modules/Mod1/Mod1", result[0].DefaultTemplate.Id);
            Assert.AreEqual(2, result[0].Skins.Count, "Two skins expected for module");
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
            templateRepo.Setup(t => t.GetAll()).Returns(templates.Select(id => new StringTemplateInfo(id, "")));
            return templateRepo.Object;
        }
    }
}
