using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TerrificNet.ViewEngine.IO
{
    public class EmbeddedResourceFileSystem : IFileSystem
    {
        private static readonly IPathHelper PathHelper = new FileSystem.FilePathHelper();
        private readonly Assembly _assembly;
        private readonly IDictionary<string, string> _names;

        public EmbeddedResourceFileSystem(Assembly assembly)
        {
            _assembly = assembly;
            _names = _assembly.GetManifestResourceNames().ToDictionary(s => Path.Combine(s), s => s);
        }

        public string BasePath
        {
            get { return string.Empty; }
        }

        public bool DirectoryExists(string directory)
        {
            return _names.ContainsKey(directory);
        }

        public IEnumerable<string> DirectoryGetFiles(string directory, string fileExtension)
        {
            throw new NotSupportedException();
        }

        public Stream OpenRead(string filePath)
        {
            string resourceName;
            if (!_names.TryGetValue(filePath, out resourceName))
                throw new ArgumentException("Invalid file path");

            return _assembly.GetManifestResourceStream(resourceName);
        }

        public Stream OpenWrite(string filePath)
        {
            throw new NotSupportedException();
        }

        public bool FileExists(string filePath)
        {
            return _names.ContainsKey(filePath);
        }

        public void RemoveFile(string filePath)
        {
            throw new NotSupportedException();
        }

        public void CreateDirectory(string directory)
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

	    public Task<IDisposable> SubscribeAsync(string pattern, Action<string> handler)
	    {
			throw new NotSupportedException();
	    }

	    public bool SupportsSubscribe
	    {
		    get { return false; }
	    }

        public string GetETag(string filePath)
        {
            return _assembly.GetName().Version.ToString(4);
        }
    }
}