using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TerrificNet.ViewEngine.IO
{
    public interface IFileSystem
    {
        string BasePath { get; }

        bool DirectoryExists(string directory);
        IEnumerable<string> DirectoryGetFiles(string directory, string fileExtension);
		Stream OpenRead(string filePath);
        Stream OpenWrite(string filePath);
        bool FileExists(string filePath);
		void RemoveFile(string filePath);
        void CreateDirectory(string directory);
	    Stream OpenReadOrCreate(string filePath);
        IPathHelper Path { get; }
        string GetETag(string filePath);
		Task<IDisposable> SubscribeAsync(string pattern, Action<string> handler);
		bool SupportsSubscribe { get; }
    }
}