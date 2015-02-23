using System.Collections.Generic;
using System.IO;

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
    }

    public interface IPathHelper
    {
        string Combine(params string[] parts);
        string GetDirectoryName(string filePath);
        string ChangeExtension(string fileName, string extension);
        string GetFileNameWithoutExtension(string path);
        string GetExtension(string path);
    }
}