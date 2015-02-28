using System.IO;

namespace TerrificNet.ViewEngine
{
    public class StringTemplateInfo : TemplateInfo
    {
        private readonly string _content;

        public StringTemplateInfo(string id, string content)
            : base(id)
        {
            _content = content;
        }

        public override Stream Open()
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(_content);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}