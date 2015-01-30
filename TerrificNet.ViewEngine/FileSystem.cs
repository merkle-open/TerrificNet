using System.Collections.Generic;
using System.IO;

namespace TerrificNet.ViewEngine
{
    public class FileSystem : IFileSystem
    {
        private static readonly IPathHelper PathHelper = new FilePathHelper();

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

		public Stream OpenReadOrCreate(string filePath)
		{
			return new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read);
		}

        public IPathHelper Path
        {
            get { return PathHelper; }
        }

        public Stream OpenWrite(string filePath)
        {
			var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            stream.SetLength(0);
            return stream;
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

	    public void RemoveFile(string filePath)
	    {
		    File.Delete(filePath);
	    }

        public void CreateDirectory(string directory)
        {
            Directory.CreateDirectory(directory);
        }

        internal class FilePathHelper : IPathHelper
        {
            public string Combine(params string[] parts)
            {
                return System.IO.Path.Combine(parts);
            }

            public string GetDirectoryName(string filePath)
            {
                return System.IO.Path.GetDirectoryName(filePath);
            }

            public string ChangeExtension(string fileName, string extension)
            {
                return System.IO.Path.ChangeExtension(fileName, extension);
            }

            public string GetFileNameWithoutExtension(string path)
            {
                return System.IO.Path.GetFileNameWithoutExtension(path);
            }

            public string GetExtension(string path)
            {
                return System.IO.Path.GetExtension(path);
            }
        }
    }
}