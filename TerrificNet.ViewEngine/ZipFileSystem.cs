using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;

namespace TerrificNet.ViewEngine
{
    public class ZipFileSystem : IFileSystem
    {
        private readonly ZipFile _file;

        public ZipFileSystem(string filePath)
        {
            _file = new ZipFile(filePath);
        }

        public bool DirectoryExists(string directory)
        {
            var entryName = NormalizePath(directory);
            return _file.OfType<ZipEntry>().Any(e => e.Name.StartsWith(entryName));
        }

        private static string NormalizePath(string directory)
        {
            return directory.TrimStart('/').Replace('\\', '/');
        }

        public IEnumerable<string> DirectoryGetFiles(string directory, string fileExtension)
        {
            return _file.OfType<ZipEntry>()
                .Where(e => e.IsFile && e.Name.StartsWith(NormalizePath(directory)) && e.Name.EndsWith(string.Concat(".", fileExtension)))
                .Select(e => e.Name);
        }

		public Stream OpenRead(string filePath)
        {
            var file = _file.GetEntry(NormalizePath(filePath));
            return _file.GetInputStream(file);
        }

		public Stream OpenWrite(string filePath)
        {
            throw new System.NotImplementedException();
        }

        public bool FileExists(string filePath)
        {
            var dir = _file.GetEntry(NormalizePath(filePath));
            return dir != null;
        }
    }
}