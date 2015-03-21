using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TerrificNet.ViewEngine.IO
{
	internal class LookupFileSystem : IDisposable
	{
		private readonly PathInfo _basePath;
        private HashSet<PathInfo> _fileInfos;
        private HashSet<PathInfo> _directoryInfos;
		private FileSystemWatcher _watcher;
		private readonly List<LookupFileSystemSubscription> _subscriptions = new List<LookupFileSystemSubscription>();

		public LookupFileSystem(PathInfo pathInfo)
		{
			if (pathInfo == null)
                throw new ArgumentNullException("pathInfo");

			_basePath = pathInfo;
			Initialize();
			InitializeWatcher();
		}

        public bool DirectoryExists(PathInfo directory)
		{
			return directory == null || _directoryInfos.Contains(directory);
		}

        public bool FileExists(PathInfo filePath)
		{
			return _fileInfos.Contains(filePath);
		}

        public IEnumerable<PathInfo> DirectoryGetFiles(PathInfo directory, string fileExtension)
		{
			var checkDirectory = directory == null;
			var checkExtension = fileExtension == null;
			if (!checkExtension)
				fileExtension = string.Concat(".", fileExtension);

			return
				_fileInfos.Where(
					f => (checkDirectory || f.StartsWith(directory)) &&
					     (checkExtension || f.HasExtension(fileExtension)));
		}

		private void Initialize()
		{
            _fileInfos = new HashSet<PathInfo>(
                Directory.EnumerateFiles(_basePath.ToString(), "*", SearchOption.AllDirectories)
                    .Select(fileName => PathInfo.GetSubPath(_basePath, fileName)));

			_directoryInfos = new HashSet<PathInfo>(
                Directory.EnumerateDirectories(_basePath.ToString(), "*", SearchOption.AllDirectories)
                    .Select(fileName => PathInfo.GetSubPath(_basePath, fileName)));
		}

		private void InitializeWatcher()
		{
			_watcher = new FileSystemWatcher(_basePath.ToString())
			{
				Path = _basePath.ToString(),
				EnableRaisingEvents = true,
				IncludeSubdirectories = true
			};
			_watcher.Changed += (sender, args) => { Initialize(); NotifySubscriptions(args.FullPath); };
			_watcher.Created += (sender, args) => { Initialize(); NotifySubscriptions(args.FullPath); };
			_watcher.Deleted += (sender, args) => { Initialize(); NotifySubscriptions(args.FullPath); };
			_watcher.Renamed += (sender, args) => { Initialize(); NotifySubscriptions(args.FullPath); };
		}

		private void NotifySubscriptions(string file)
		{
			foreach (var subscription in _subscriptions)
			{
				subscription.Notify(file);
			}
		}

		public Task<IDisposable> SubscribeAsync(string pattern, Action<string> handler)
		{
			var subscription = new LookupFileSystemSubscription(this, handler);
			_subscriptions.Add(subscription);

			return Task.FromResult<IDisposable>(subscription);
		}

		private void Unsubscribe(LookupFileSystemSubscription subscription)
		{
			_subscriptions.Remove(subscription);
		}

		public void Dispose()
		{
			_watcher.Dispose();
		}

		private class LookupFileSystemSubscription : IDisposable
		{
			private LookupFileSystem _parent;
			private Action<string> _handler;

			public LookupFileSystemSubscription(LookupFileSystem parent, Action<string> handler)
			{
				_parent = parent;
				_handler = handler;
			}

			internal void Notify(string file)
			{
				_handler(file);
			}

			public void Dispose()
			{
				_parent.Unsubscribe(this);
				_handler = null;
				_parent = null;
			}
		}
	}
}