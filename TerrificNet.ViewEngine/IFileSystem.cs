using System;
using System.Collections.Generic;
using System.IO;

namespace TerrificNet.ViewEngine
{
    public interface IFileSystem
    {
        bool DirectoryExists(string directory);
        IEnumerable<string> DirectoryGetFiles(string directory, string fileExtension);
        StreamReader OpenRead(string filePath);
        StreamWriter OpenWrite(string filePath);
        bool FileExists(string filePath);
    }
}