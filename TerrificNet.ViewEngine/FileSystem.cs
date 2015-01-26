using System.Collections.Generic;
using System.IO;

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
}