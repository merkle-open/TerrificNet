using System;
using System.Collections.Generic;
using System.IO;

namespace TerrificNet.ViewEngine
{
    public interface IFileSystem
    {
        bool DirectoryExists(string directory);
        IEnumerable<string> DirectoryGetFiles(string directory, string fileExtension);
		Stream OpenRead(string filePath);
        Stream OpenWrite(string filePath);
        bool FileExists(string filePath);
		void RemoveFile(string filePath);
	    Stream OpenReadOrCreate(string filePath);
    }
}