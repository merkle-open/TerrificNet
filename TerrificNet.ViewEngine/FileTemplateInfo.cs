using System.IO;

namespace TerrificNet.ViewEngine
{
    internal class FileTemplateInfo : TemplateInfo
    {
        private readonly string _filePath;
        private readonly IFileSystem _fileSystem;

        public FileTemplateInfo(string id, string filePath, IFileSystem fileSystem) : base(id)
        {
            _filePath = filePath;
            _fileSystem = fileSystem;
        }

        public override Stream Open()
        {
            return _fileSystem.OpenRead(_filePath).BaseStream;
        }
    }
}
