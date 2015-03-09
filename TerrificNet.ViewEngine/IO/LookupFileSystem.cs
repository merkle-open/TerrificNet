using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TerrificNet.ViewEngine.IO
{
	internal class LookupFileSystem : IDisposable
	{
		private readonly string _basePath;
		private HashSet<string> _fileInfos;
		private HashSet<string> _directoryInfos;
		private FileSystemWatcher _watcher;
		private readonly List<LookupFileSystemSubscription> _subscriptions = new List<LookupFileSystemSubscription>();

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
			_watcher = new FileSystemWatcher(_basePath)
			{
				Path = _basePath,
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

		public Task<IFileSystemSubscription> SubscribeAsync(string pattern)
		{
			var subscription = new LookupFileSystemSubscription(this);
			_subscriptions.Add(subscription);
			return Task.FromResult((IFileSystemSubscription)subscription);
		}

		internal void Unsubscribe(IFileSystemSubscription subscription)
		{
			_subscriptions.Remove((LookupFileSystemSubscription)subscription);
		}

		public void Dispose()
		{
			_watcher.Dispose();
		}

		private class LookupFileSystemSubscription : IFileSystemSubscription
		{
			private readonly LookupFileSystem _parent;
			private readonly List<Action<string>> _handlers = new List<Action<string>>();

			public LookupFileSystemSubscription(LookupFileSystem parent)
			{
				_parent = parent;
			}

			public void Register(Action<string> handler)
			{
				_handlers.Add(handler);
			}

			internal void Notify(string file)
			{
				foreach (var handler in _handlers)
				{
					handler(file);
				}
			}

			public void Dispose()
			{
				_parent.Unsubscribe(this);
			}
		}
	}
}