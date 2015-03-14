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
		private readonly Func<List<FileTemplateInfo>> _getAll;
		private readonly IDisposable _subscription;

		public TerrificTemplateRepository(IFileSystem fileSystem)
		{
			_fileSystem = fileSystem;

			_getAll = () => _fileSystem.DirectoryGetFiles(null, "html").Select(f =>
			{
				var relativePath = GetTemplateId(f).TrimStart('/');
				return new FileTemplateInfo(relativePath, f, _fileSystem);
			}).ToList();

			if (!_fileSystem.SupportsSubscribe) 
				return;

			var cache = _getAll();
			_subscription = _fileSystem.SubscribeAsync("*.html", s => { cache = _getAll(); }).Result;
			
			_getAll = () => cache;
		}

		public Task<TemplateInfo> GetTemplateAsync(string id)
		{
			var fileName = id;
            // TODO: use async
			var templates = GetAll().ToDictionary(f => f.Id, f => f);
		    TemplateInfo templateInfo;
		    if (templates.TryGetValue(fileName, out templateInfo))
		        return Task.FromResult(templateInfo);

		    return Task.FromResult<TemplateInfo>(null);
		}

		private string GetTemplateId(string info)
		{
			return _fileSystem.Path.Combine(_fileSystem.Path.GetDirectoryName(info), _fileSystem.Path.GetFileNameWithoutExtension(info));
		}

		public IEnumerable<TemplateInfo> GetAll()
		{
			return _getAll();
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
		}
	}
}