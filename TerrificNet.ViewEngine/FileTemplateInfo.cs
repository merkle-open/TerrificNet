using System.IO;

namespace TerrificNet.ViewEngine
{
    internal class FileTemplateInfo : TemplateInfo
    {
        private readonly FileInfo _file;

        public FileTemplateInfo(string id, FileInfo file) : base(id)
        {
            _file = file;
        }

        public override Stream Open()
        {
            return _file.OpenRead();
        }
    }
}
