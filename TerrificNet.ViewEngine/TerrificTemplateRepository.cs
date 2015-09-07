using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TerrificNet.ViewEngine.Config;
using TerrificNet.ViewEngine.IO;

namespace TerrificNet.ViewEngine
{
	public class TerrificTemplateRepository : ITemplateRepository, IDisposable
	{
		private readonly IFileSystem _fileSystem;
        private readonly ITerrificNetConfig _configuration;
		private Dictionary<string, FileTemplateInfo> _templates;
		private IDisposable _moduleSubscription;
        private IDisposable _viewSubscription;

        public TerrificTemplateRepository(IFileSystem fileSystem, ITerrificNetConfig configuration)
		{
			_fileSystem = fileSystem;
            _configuration = configuration;

			InitCache();

			if (!_fileSystem.SupportsSubscribe)
				return;

            _moduleSubscription =
                _fileSystem.SubscribeDirectoryGetFilesAsync(PathInfo.Create(_configuration.ModulePath.ToString()),
                    "html", files => InitCache());

            _viewSubscription =
                _fileSystem.SubscribeDirectoryGetFilesAsync(PathInfo.Create(_configuration.ViewPath.ToString()),
                    "html", files => InitCache());
		}

		private void InitCache()
		{
		    var moduleTemplates = _fileSystem.DirectoryGetFiles(_configuration.ModulePath, "html").Select(f =>
		    {
		        var relativePath = GetTemplateId(f).RemoveStartSlash();
		        return new FileTemplateInfo(relativePath.ToString(), f, _fileSystem);
		    });

		    var viewTemplates = _fileSystem.DirectoryGetFiles(_configuration.ViewPath, "html").Select(f =>
		    {
		        var relativePath = GetTemplateId(f).RemoveStartSlash();
		        return new FileTemplateInfo(relativePath.ToString(), f, _fileSystem);
		    });

            _templates = moduleTemplates.Concat(viewTemplates).ToDictionary(i => i.Id, i => i);
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
		    {
		        _moduleSubscription.Dispose();
                _viewSubscription.Dispose();
		    }

		    _moduleSubscription = null;
		    _viewSubscription = null;
		}
	}
}