using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TerrificNet.ViewEngine.IO;

namespace TerrificNet.ViewEngine
{
	public class TerrificTemplateRepository : ITemplateRepository, IDisposable
	{
		private readonly IFileSystem _fileSystem;
		private Dictionary<string, FileTemplateInfo> _templates;
		private IDisposable _subscription;

		public TerrificTemplateRepository(IFileSystem fileSystem)
		{
			_fileSystem = fileSystem;

			InitCache();

			if (!_fileSystem.SupportsSubscribe)
				return;

			_fileSystem.SubscribeDirectoryGetFilesAsync(PathInfo.Create(""), "html", files => InitCache());
		}

		private void InitCache()
		{
			_templates = _fileSystem.DirectoryGetFiles(null, "html").Select(f =>
			{
				var relativePath = GetTemplateId(f).RemoveStartSlash();
				return new FileTemplateInfo(relativePath.ToString(), f, _fileSystem);
			}).ToDictionary(i => i.Id, i => i);
		}

		public Task<TemplateInfo> GetTemplateAsync(string id)
		{
			FileTemplateInfo templateInfo;
			if (_templates.TryGetValue(id, out templateInfo))
				return Task.FromResult<TemplateInfo>(templateInfo);

			return Task.FromResult<TemplateInfo>(null);
		}

		private PathInfo GetTemplateId(PathInfo info)
		{
			return _fileSystem.Path.Combine(_fileSystem.Path.GetDirectoryName(info), _fileSystem.Path.GetFileNameWithoutExtension(info));
		}

		public IEnumerable<TemplateInfo> GetAll()
		{
			return _templates.Values;
		}

		~TerrificTemplateRepository()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
				_subscription.Dispose();

			_subscription = null;
		}
	}
}