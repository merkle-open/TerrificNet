using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TerrificNet.ViewEngine.IO
{
    public class FileSystem : IFileSystem
    {
        private readonly string _basePath;
        private static readonly IPathHelper PathHelper = new FilePathHelper();
        private readonly Func<string, bool> _directoryExistsAction;
        private readonly Func<string, string, IEnumerable<string>> _directoryGetFilesAction;
        private readonly Func<string, bool> _fileExistsAction;
        private readonly LookupFileSystem _lookupSystem;

        public FileSystem() : this(string.Empty, false)
        {
        }

        public FileSystem(string basePath) 
			: this(basePath, false)
        {
        }

        public FileSystem(string basePath, bool useCache)
        {
            _basePath = PathUtility.Combine(basePath);

            if (!useCache || string.IsNullOrEmpty(basePath))
            {
                _directoryExistsAction = directory => Directory.Exists(GetRootPath(directory));
                _directoryGetFilesAction = (directory, fileExtension) => Directory.EnumerateFiles(GetRootPath(directory), string.Concat("*.", fileExtension), SearchOption.AllDirectories).Select(fileName => PathUtility.Combine(fileName.Substring(_basePath.Length)));
                _fileExistsAction = filePath => File.Exists(GetRootPath(filePath));
            }
            else
            {
                _lookupSystem = new LookupFileSystem(_basePath);
                _directoryExistsAction = directory => _lookupSystem.DirectoryExists(directory);
                _directoryGetFilesAction = (directory, fileExtension) => _lookupSystem.DirectoryGetFiles(directory, fileExtension);
                _fileExistsAction = filePath => _lookupSystem.FileExists(filePath);
            }
        }

        public string BasePath { get { return _basePath; } }

        public bool DirectoryExists(string directory)
        {
            return _directoryExistsAction(directory);
        }

        public IEnumerable<string> DirectoryGetFiles(string directory, string fileExtension)
        {
            return _directoryGetFilesAction(directory, fileExtension);
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

        public string GetETag(string filePath)
        {
            return new FileInfo(GetRootPath(filePath)).LastWriteTimeUtc.Ticks.ToString("X8");
        }

        public Stream OpenWrite(string filePath)
        {
            var stream = new FileStream(GetRootPath(filePath), FileMode.OpenOrCreate, FileAccess.Write);
            stream.SetLength(0);
            return stream;
        }

        public bool FileExists(string filePath)
        {
            return _fileExistsAction(filePath);
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
            if (string.IsNullOrEmpty(part))
                return _basePath;

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