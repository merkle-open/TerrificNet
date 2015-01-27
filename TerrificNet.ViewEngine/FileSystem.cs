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

        public Stream OpenRead(string filePath)
        {
            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }

		public Stream OpenWrite(string filePath)
        {
			return new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}