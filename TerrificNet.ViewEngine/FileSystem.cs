using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;

namespace TerrificNet.ViewEngine
{
    public class FileSystem : IFileSystem
    {
        public bool DirectoryExists(string directory)
        {
            return Directory.Exists(directory);
        }

        public IEnumerable<string> DirectoryGetFiles(string directory, string fileExtension)
        {
            return Directory.GetFiles(directory, string.Concat("*.", fileExtension), SearchOption.AllDirectories);
        }

        public StreamReader OpenRead(string filePath)
        {
            return new StreamReader(filePath);
        }

        public StreamWriter OpenWrite(string filePath)
        {
            return new StreamWriter(filePath);
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }
    }

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
            //var dir = _file.GetEntry(directory);
            //return dir != null;
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

        public StreamReader OpenRead(string filePath)
        {
            var file = _file.GetEntry(NormalizePath(filePath));
            return new StreamReader(_file.GetInputStream(file));
        }

        public StreamWriter OpenWrite(string filePath)
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