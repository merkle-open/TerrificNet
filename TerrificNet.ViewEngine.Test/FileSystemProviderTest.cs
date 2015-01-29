using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TerrificNet.ViewEngine.Test
{
    [TestClass]
    public class FileSystemProviderTest
    {
        [TestMethod]
        public void TestZipRelativeUrl()
        {
            TestZip("C:\\test", "zip://web.zip/", "C:\\test\\web.zip", "");
        }

        [TestMethod]
        public void TestZipRelativeWithDotsUrl()
        {
            TestZip("C:\\test", "zip://../web.zip/", "C:\\web.zip", "");
        }

        [TestMethod]
        public void TestZipInlinePath()
        {
            TestZip("C:\\test", "zip://../web.zip/inlinezip", "C:\\web.zip", "inlinezip/");
        }

        [TestMethod]
        public void TestZipAbsolutePath()
        {
            TestZip("C:\\test", "zip://d:\\data\\web.zip\\", "d:\\data\\web.zip", "");
        }

        [TestMethod]
        public void TestMacPath()
        {
            TestZip("C:\\test", "zip://../web.zip/", "C:\\web.zip", "");
        }

        private static void TestZip(string hostPath, string expression, string expectedPath, string expectedInnerPath)
        {
            var underTest = new Mock<FileSystemProvider>();
            underTest.Setup(u => u.CreateZipFileSystem(It.Is<string>(filePath => filePath == expectedPath)))
                .Returns((ZipFileSystem) null);

            string basePath;
            var result = underTest.Object.GetFileSystem(hostPath, expression, out basePath);

            underTest.VerifyAll();

            Assert.AreEqual(basePath, expectedInnerPath);
        }
    }
}
