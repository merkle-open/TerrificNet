using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TerrificNet.ViewEngine.IO
{
    internal class LookupFileSystem
    {
        private readonly string _basePath;
        private HashSet<string> _fileInfos;
        private HashSet<string> _directoryInfos;

        public LookupFileSystem(string basePath)
        {
            if (string.IsNullOrEmpty(basePath))
                throw new ArgumentNullException("basePath");

            _basePath = basePath;
            Initialize();
            InitializeWatcher();
        }

        public bool DirectoryExists(string directory)
        {
            return string.IsNullOrEmpty(directory) || _directoryInfos.Contains(directory);
        }

        public bool FileExists(string filePath)
        {
            return _fileInfos.Contains(filePath);
        }

        public IEnumerable<string> DirectoryGetFiles(string directory, string fileExtension)
        {
            var checkDirectory = directory == null;
            var checkExtension = fileExtension == null;
            if (!checkExtension)
                fileExtension = string.Concat(".", fileExtension);

            return
                _fileInfos.Where(
                    f =>
                    {
                        return (checkDirectory || f.StartsWith(directory, StringComparison.InvariantCultureIgnoreCase)) &&
                               (checkExtension || f.EndsWith(fileExtension));
                    });
        }

        private void Initialize()
        {
            _fileInfos = new HashSet<string>(Directory.EnumerateFiles(_basePath, "*", SearchOption.AllDirectories).Select(fileName => PathUtility.Combine(fileName.Substring(_basePath.Length + 1))));
            _directoryInfos = new HashSet<string>(Directory.EnumerateDirectories(_basePath, "*", SearchOption.AllDirectories).Select(fileName => PathUtility.Combine(fileName.Substring(_basePath.Length + 1))));
        }

        private void InitializeWatcher()
        {
            var watcher = new FileSystemWatcher(_basePath)
            {
                Path = _basePath,
                EnableRaisingEvents = true,
                IncludeSubdirectories = true
            };
            watcher.Changed += (sender, args) => { Initialize(); };
            watcher.Created += (sender, args) => { Initialize(); };
            watcher.Deleted += (sender, args) => { Initialize(); };
            watcher.Renamed += (sender, args) => { Initialize(); };
        }
    }
}