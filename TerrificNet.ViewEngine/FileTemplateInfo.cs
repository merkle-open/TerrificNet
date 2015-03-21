using System.IO;
using TerrificNet.ViewEngine.IO;

namespace TerrificNet.ViewEngine
{
    internal class FileTemplateInfo : TemplateInfo
    {
        private readonly PathInfo _filePath;
        private readonly IFileSystem _fileSystem;

        public FileTemplateInfo(string id, PathInfo filePath, IFileSystem fileSystem)
            : base(id)
        {
            _filePath = filePath;
            _fileSystem = fileSystem;
        }

        public override Stream Open()
        {
            return _fileSystem.OpenRead(_filePath);
        }

        public override string ETag
        {
            get { return _fileSystem.GetETag(_filePath); }
        }
    }
}
