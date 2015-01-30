using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TerrificNet.ViewEngine
{
    public class FileSystem : IFileSystem
    {
        private readonly string _basePath;
        private static readonly IPathHelper PathHelper = new FilePathHelper();

        public FileSystem() : this(string.Empty)
        {
        }

        public FileSystem(string basePath)
        {
            _basePath = PathUtility.Combine(basePath);
        }

        public string BasePath { get { return _basePath; } }

        public bool DirectoryExists(string directory)
        {
            return Directory.Exists(GetRootPath(directory));
        }

        public IEnumerable<string> DirectoryGetFiles(string directory, string fileExtension)
        {
            return Directory.GetFiles(GetRootPath(directory), string.Concat("*.", fileExtension), SearchOption.AllDirectories)
                .Select(fileName => PathUtility.Combine(fileName.Substring(_basePath.Length)));
        }

        public Stream OpenRead(string filePath)
        {
            return new FileStream(GetRootPath(filePath), FileMode.Open, FileAccess.Read);
        }

		public Stream OpenReadOrCreate(string filePath)
		{
            return new FileStream(GetRootPath(filePath), FileMode.OpenOrCreate, FileAccess.Read);
		}

        public IPathHelper Path
        {
            get { return PathHelper; }
        }

        public Stream OpenWrite(string filePath)
        {
            var stream = new FileStream(GetRootPath(filePath), FileMode.OpenOrCreate, FileAccess.Write);
            stream.SetLength(0);
            return stream;
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(GetRootPath(filePath));
        }

	    public void RemoveFile(string filePath)
	    {
            File.Delete(GetRootPath(filePath));
	    }

        public void CreateDirectory(string directory)
        {
            Directory.CreateDirectory(GetRootPath(directory));
        }

        private string GetRootPath(string part)
        {
            return Path.Combine(_basePath, part.TrimStart('/'));
        }


        internal class FilePathHelper : IPathHelper
        {
            public string Combine(params string[] parts)
            {
                return PathUtility.Combine(parts);
            }

            public string GetDirectoryName(string filePath)
            {
                return PathUtility.GetDirectoryName(filePath);
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