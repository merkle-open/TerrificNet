using System;
using System.Collections.Generic;
using System.Linq;
using TerrificNet.ViewEngine.IO;
using Veil;

namespace TerrificNet.ViewEngine
{
	public class TerrificTemplateRepository : ITemplateRepository, IDisposable
	{
		private readonly IFileSystem _fileSystem;
		private readonly Func<List<FileTemplateInfo>> _getAll;
		private readonly IFileSystemSubscription _subscription;

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

			_subscription = _fileSystem.SubscribeAsync("*.html").Result;
			var cache = _getAll();
			_subscription.Register(s => { cache = _getAll(); });
			_getAll = () => cache;
		}

		public bool TryGetTemplate(string id, out TemplateInfo templateInfo)
		{
			var fileName = id;
			var templates = GetAll().ToDictionary(f => f.Id, f => f);
			return templates.TryGetValue(fileName, out templateInfo);
		}

		private string GetTemplateId(string info)
		{
			return _fileSystem.Path.Combine(_fileSystem.Path.GetDirectoryName(info), _fileSystem.Path.GetFileNameWithoutExtension(info));
		}

		public IEnumerable<TemplateInfo> GetAll()
		{
			return _getAll();
		}

		public void Dispose()
		{
			_subscription.Dispose();
		}
	}
}