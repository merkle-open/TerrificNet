using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;

namespace TerrificNet.ViewEngine
{
    public class ZipFileSystem : IFileSystem
    {
        private static readonly IPathHelper PathHelper = new ZipPathHelper();
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

	    public void RemoveFile(string filePath)
	    {
		    throw new System.NotImplementedException();
	    }

	    public Stream OpenReadOrCreate(string filePath)
	    {
		    throw new System.NotImplementedException();
	    }

        public IPathHelper Path
        {
            get { return PathHelper; }
        }

        public void CreateDirectory(string directory)
        {
            throw new NotSupportedException();
        }

        private class ZipPathHelper : IPathHelper
        {
            public string Combine(params string[] parts)
            {
                return string.Join("/", parts);
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