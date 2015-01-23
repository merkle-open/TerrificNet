using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TerrificNet.ViewEngine.Config;

namespace TerrificNet.ViewEngine
{
	public class TerrificTemplateRepository : ITemplateRepository
	{
		private const string HtmlExtension = "html";

		private readonly ITerrificNetConfig _config;

		public TerrificTemplateRepository(ITerrificNetConfig config)
		{
			_config = config;
		}

		public bool TryGetTemplate(string id, string skin, out TemplateInfo templateInfo)
		{
		    var fileName = id;
			if (!string.IsNullOrEmpty(skin))
				fileName += "-" + skin;

		    var templates = GetAll().ToDictionary(f => f.Id, f => f);
		    return templates.TryGetValue(fileName, out templateInfo);
		}

	    private IEnumerable<TemplateInfo> Read(string directory)
	    {
	        if (!Directory.Exists(directory))
	            return Enumerable.Empty<TemplateInfo>();

			return Directory.GetFiles(directory, "*.html", SearchOption.AllDirectories).Select(f =>
			{
			    var info = new FileInfo(f);
			    var relativePath = GetTemplateId(info);
                return new FileTemplateInfo(relativePath, info); 
            });
	    }

	    private string GetTemplateId(FileInfo info)
	    {
	        var path = Path.Combine(info.DirectoryName, Path.GetFileNameWithoutExtension(info.Name));
            var id = new Uri(path).AbsoluteUri.Remove(0, new Uri(_config.BasePath).AbsoluteUri.Length);
	        if (id[0] == '/')
	            return id.Substring(1);

            return id;
	    }

	    public IEnumerable<TemplateInfo> GetAll()
	    {
	        return Read(_config.ViewPath)
                .Union(Read(_config.ModulePath));
	    }
	}
}