using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TerrificNet.ViewEngine.IO
{
    public interface IFileSystem
    {
        PathInfo BasePath { get; }

        bool DirectoryExists(PathInfo directory);
        IEnumerable<PathInfo> DirectoryGetFiles(PathInfo directory, string fileExtension);
        Stream OpenRead(PathInfo filePath);
        Stream OpenWrite(PathInfo filePath);
        bool FileExists(PathInfo filePath);
		void RemoveFile(PathInfo filePath);
        void CreateDirectory(PathInfo directory);
	    Stream OpenReadOrCreate(PathInfo filePath);
        IPathHelper Path { get; }
        string GetETag(PathInfo filePath);
		Task<IDisposable> SubscribeAsync(string pattern, Action<string> handler);
		bool SupportsSubscribe { get; }
    }
}