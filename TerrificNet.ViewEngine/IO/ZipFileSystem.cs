using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;

namespace TerrificNet.ViewEngine.IO
{
    public class ZipFileSystem : IFileSystem
    {
        private readonly string _filePath;
        private readonly string _rootPath;
        private static readonly IPathHelper PathHelper = new ZipPathHelper();
        private readonly ZipFile _file;
        private readonly string _etag;

        public ZipFileSystem(string filePath, string rootPath)
        {
            _filePath = filePath;
            _rootPath = rootPath;
            _file = new ZipFile(filePath);
            _etag = new FileInfo(filePath).LastWriteTimeUtc.Ticks.ToString("X8");
        }

        public string BasePath { get { return _filePath; } }

        public bool DirectoryExists(string directory)
        {
            var entryName = GetFullPath(directory);
            return _file.OfType<ZipEntry>().Any(e => e.Name.StartsWith(entryName));
        }

        public IEnumerable<string> DirectoryGetFiles(string directory, string fileExtension)
        {
            return _file.OfType<ZipEntry>()
                .Where(e => e.IsFile && e.Name.StartsWith(GetFullPath(directory)) && e.Name.EndsWith(string.Concat(".", fileExtension)))
                .Select(e => e.Name.Substring(_rootPath.Length));
        }

		public Stream OpenRead(string filePath)
        {
            var file = _file.GetEntry(GetFullPath(filePath));
            return _file.GetInputStream(file);
        }

		public Stream OpenWrite(string filePath)
        {
            throw new NotSupportedException();
        }

        public bool FileExists(string filePath)
        {
            var dir = _file.GetEntry(GetFullPath(filePath));
            return dir != null;
        }

	    public void RemoveFile(string filePath)
	    {
	        throw new NotSupportedException();
	    }

	    public Stream OpenReadOrCreate(string filePath)
	    {
            throw new NotSupportedException();
	    }

        public IPathHelper Path
        {
            get { return PathHelper; }
        }

        public string GetETag(string filePath)
        {
            return _etag;
        }

	    public Task<IFileSystemSubscription> SubscribeAsync(string pattern)
	    {
			throw new NotSupportedException();
	    }

	    public bool SupportsSubscribe
	    {
		    get { return false; }
	    }

	    public void CreateDirectory(string directory)
        {
            throw new NotSupportedException();
        }

        private string GetFullPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return _rootPath;

            return PathUtility.Combine(_rootPath, path);
        }

        private class ZipPathHelper : IPathHelper
        {
            public string Combine(params string[] parts)
            {
                return PathUtility.Combine(parts);
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