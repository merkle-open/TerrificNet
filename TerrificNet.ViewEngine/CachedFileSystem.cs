using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace TerrificNet.ViewEngine
{
	public class CachedFileSystem : IFileSystem
	{
		private readonly ConcurrentDictionary<string, IEnumerable<string>> _directoryCache = new ConcurrentDictionary<string, IEnumerable<string>>(); 
		private readonly ConcurrentDictionary<string, byte[]> _filesContentCache = new ConcurrentDictionary<string, byte[]>(); 

		public bool DirectoryExists(string directory)
		{
			return Directory.Exists(directory);
		}

		public IEnumerable<string> DirectoryGetFiles(string directory, string fileExtension)
		{
			return _directoryCache.GetOrAdd(string.Join("|", directory, string.Concat("*.", fileExtension)), key =>
			{
				var watcher = new FileSystemWatcher(directory)
				{
					Path = directory,
					EnableRaisingEvents = true,
					IncludeSubdirectories = true
				};
				watcher.Changed += (sender, args) => { ClearDirectoryCache(key); };
				watcher.Created += (sender, args) => { ClearDirectoryCache(key); };
				watcher.Deleted += (sender, args) => { ClearDirectoryCache(key); };
				watcher.Renamed += (sender, args) => { ClearDirectoryCache(key); };

				return Directory.GetFiles(directory, string.Concat("*.", fileExtension), SearchOption.AllDirectories);
			});
		}

		private void ClearDirectoryCache(string key)
		{
			IEnumerable<string> tmp;
			_directoryCache.TryRemove(key, out tmp);
		}

		public Stream OpenRead(string filePath)
		{
			var data = _filesContentCache.GetOrAdd(filePath, file =>
			{
				var buffer = new MemoryStream();
				new FileStream(filePath, FileMode.Open, FileAccess.Read).CopyTo(buffer);
				return buffer.ToArray();
			});
			return new MemoryStream(data);
		}

		public Stream OpenWrite(string filePath)
		{
			return new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
		}

		public bool FileExists(string filePath)
		{
			return File.Exists(filePath);
		}
	}
}