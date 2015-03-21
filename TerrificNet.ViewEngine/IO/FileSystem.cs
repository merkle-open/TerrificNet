using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TerrificNet.ViewEngine.IO
{
	public class FileSystem : IFileSystem
	{
		private static readonly IPathHelper PathHelper = new FilePathHelper();

		private readonly PathInfo _basePath;
        private readonly Func<PathInfo, bool> _directoryExistsAction;
        private readonly Func<PathInfo, string, IEnumerable<PathInfo>> _directoryGetFilesAction;
		private readonly Func<PathInfo, bool> _fileExistsAction;
		private readonly LookupFileSystem _lookupSystem;

		public FileSystem()
			: this(string.Empty, false)
		{
		}

		public FileSystem(string basePath)
			: this(basePath, false)
		{
		}

		public FileSystem(string basePath, bool useCache)
		{
			_basePath = PathInfo.Create(PathUtility.Combine(basePath));

			if (!useCache || string.IsNullOrEmpty(basePath))
			{
				_directoryExistsAction = directory => Directory.Exists(GetRootPath(directory).ToString());
				_directoryGetFilesAction = (directory, fileExtension) => Directory.EnumerateFiles(GetRootPath(directory).ToString(), string.Concat("*.", fileExtension), SearchOption.AllDirectories).Select(fileName => PathInfo.Create(PathUtility.Combine(fileName.Substring(_basePath.ToString().Length))));
				_fileExistsAction = filePath => File.Exists(GetRootPath(filePath).ToString());
			}
			else
			{
				_lookupSystem = new LookupFileSystem(_basePath.ToString());
				_directoryExistsAction = directory => _lookupSystem.DirectoryExists(directory);
				_directoryGetFilesAction = (directory, fileExtension) => _lookupSystem.DirectoryGetFiles(directory, fileExtension);
				_fileExistsAction = filePath => _lookupSystem.FileExists(filePath);
			}
		}

		public PathInfo BasePath { get { return _basePath; } }

        public bool DirectoryExists(PathInfo directory)
		{
			return _directoryExistsAction(directory);
		}

        public IEnumerable<PathInfo> DirectoryGetFiles(PathInfo directory, string fileExtension)
		{
			return _directoryGetFilesAction(directory, fileExtension);
		}

		public Stream OpenRead(PathInfo filePath)
		{
			return new FileStream(GetRootPath(filePath).ToString(), FileMode.Open, FileAccess.Read);
		}

        public Stream OpenReadOrCreate(PathInfo filePath)
		{
			return new FileStream(GetRootPath(filePath).ToString(), FileMode.OpenOrCreate, FileAccess.Read);
		}

		public IPathHelper Path
		{
			get { return PathHelper; }
		}

		public Task<IDisposable> SubscribeAsync(string pattern, Action<string> handler)
		{
			if (_lookupSystem == null)
				throw new NotSupportedException();

			return _lookupSystem.SubscribeAsync(pattern, handler);
		}

		public bool SupportsSubscribe
		{
			get { return _lookupSystem != null; }
		}

        public string GetETag(PathInfo filePath)
		{
			return new FileInfo(GetRootPath(filePath).ToString()).LastWriteTimeUtc.Ticks.ToString("X8");
		}

        public Stream OpenWrite(PathInfo filePath)
		{
			var stream = new FileStream(GetRootPath(filePath).ToString(), FileMode.OpenOrCreate, FileAccess.Write);
			stream.SetLength(0);
			return stream;
		}

		public bool FileExists(PathInfo filePath)
		{
			return _fileExistsAction(filePath);
		}

        public void RemoveFile(PathInfo filePath)
		{
			File.Delete(GetRootPath(filePath).ToString());
		}

        public void CreateDirectory(PathInfo directory)
		{
			Directory.CreateDirectory(GetRootPath(directory).ToString());
		}

		private PathInfo GetRootPath(PathInfo part)
		{
			if (part == null)
				return _basePath;

			return Path.Combine(_basePath, part.TrimStart('/'));
		}


		internal class FilePathHelper : IPathHelper
		{
			public PathInfo Combine(params PathInfo[] parts)
			{
                return PathInfo.Create(PathUtility.Combine(parts.Select(s => s == null ? null : s.ToString()).ToArray()));
			}

            public PathInfo GetDirectoryName(PathInfo filePath)
			{
				return PathInfo.Create(PathUtility.GetDirectoryName(filePath.ToString()));
			}

            public PathInfo ChangeExtension(PathInfo fileName, string extension)
			{
				return PathInfo.Create(System.IO.Path.ChangeExtension(fileName.ToString(), extension));
			}

            public PathInfo GetFileNameWithoutExtension(PathInfo path)
			{
				return PathInfo.Create(System.IO.Path.GetFileNameWithoutExtension(path.ToString()));
			}

            public string GetExtension(PathInfo path)
			{
				return System.IO.Path.GetExtension(path.ToString());
			}
		}
	}
}