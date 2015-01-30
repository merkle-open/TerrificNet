using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using Microsoft.Win32.SafeHandles;

namespace TerrificNet.ViewEngine
{
	public class CachedFileSystem : IFileSystem
	{
	    private static readonly IPathHelper PathHelper = new FileSystem.FilePathHelper();

		private readonly ConcurrentDictionary<string, IEnumerable<string>> _directoryCache = new ConcurrentDictionary<string, IEnumerable<string>>();
		private readonly ConcurrentDictionary<string, byte[]> _filesContentCache = new ConcurrentDictionary<string, byte[]>();

	    public string BasePath { get; private set; }

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
			return OpenInternal(filePath, FileMode.Open);
		}

		public Stream OpenReadOrCreate(string filePath)
		{
			return OpenInternal(filePath, FileMode.OpenOrCreate);
		}

	    public IPathHelper Path
	    {
            get { return PathHelper; }
	    }

	    private Stream OpenInternal(string filePath, FileMode fileMode)
		{
			var data = _filesContentCache.GetOrAdd(filePath, file =>
			{
				var buffer = new MemoryStream();
				using (var stream = new FileStream(filePath, fileMode, FileAccess.Read, FileShare.Read))
				{
					stream.CopyTo(buffer);
				}
				return buffer.ToArray();
			});
			return new MemoryStream(data);
		}

		public Stream OpenWrite(string filePath)
		{
			return new NotifiyFileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write)
			{
				OnDispose = () =>
				{
					byte[] tmp;
					_filesContentCache.TryRemove(filePath, out tmp);
				}
			};
		}

		public bool FileExists(string filePath)
		{
			return File.Exists(filePath);
		}

		public void RemoveFile(string filePath)
		{
			File.Delete(filePath);

			byte[] tmp;
			_filesContentCache.TryRemove(filePath, out tmp);
		}

	    public void CreateDirectory(string directory)
	    {
	        Directory.CreateDirectory(directory);
	    }

	    private class NotifiyFileStream : FileStream
		{
			public NotifiyFileStream(string path, FileMode mode)
				: base(path, mode)
			{
			}

			public NotifiyFileStream(string path, FileMode mode, FileAccess access)
				: base(path, mode, access)
			{
			}

			public NotifiyFileStream(string path, FileMode mode, FileAccess access, FileShare share)
				: base(path, mode, access, share)
			{
			}

			public NotifiyFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize)
				: base(path, mode, access, share, bufferSize)
			{
			}

			public NotifiyFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options)
				: base(path, mode, access, share, bufferSize, options)
			{
			}

			public NotifiyFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync)
				: base(path, mode, access, share, bufferSize, useAsync)
			{
			}

			public NotifiyFileStream(string path, FileMode mode, FileSystemRights rights, FileShare share, int bufferSize, FileOptions options, FileSecurity fileSecurity)
				: base(path, mode, rights, share, bufferSize, options, fileSecurity)
			{
			}

			public NotifiyFileStream(string path, FileMode mode, FileSystemRights rights, FileShare share, int bufferSize, FileOptions options)
				: base(path, mode, rights, share, bufferSize, options)
			{
			}

			public NotifiyFileStream(IntPtr handle, FileAccess access)
				: base(handle, access)
			{
			}

			public NotifiyFileStream(IntPtr handle, FileAccess access, bool ownsHandle)
				: base(handle, access, ownsHandle)
			{
			}

			public NotifiyFileStream(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize)
				: base(handle, access, ownsHandle, bufferSize)
			{
			}

			public NotifiyFileStream(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize, bool isAsync)
				: base(handle, access, ownsHandle, bufferSize, isAsync)
			{
			}

			public NotifiyFileStream(SafeFileHandle handle, FileAccess access)
				: base(handle, access)
			{
			}

			public NotifiyFileStream(SafeFileHandle handle, FileAccess access, int bufferSize)
				: base(handle, access, bufferSize)
			{
			}

			public NotifiyFileStream(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync)
				: base(handle, access, bufferSize, isAsync)
			{
			}

			public Action OnDispose { set; private get; }

			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);
				if (OnDispose != null)
					OnDispose();
			}
		}
	}
}