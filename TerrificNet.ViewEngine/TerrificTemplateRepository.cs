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

		public bool TryGetTemplate(string id, out TemplateInfo templateInfo)
		{
            templateInfo = null;

			var fileName = Path.ChangeExtension(id, HtmlExtension);

			// locate views
		    if (TryGetTemplate(id, ref templateInfo, _config.ViewPath, fileName)) 
                return true;

            if (TryGetTemplate(id, ref templateInfo, _config.ModulePath, fileName)) 
                return true;

		    return false;
		}

	    private static bool TryGetTemplate(string id, ref TemplateInfo templateInfo, string viewPath, string fileName)
	    {
	        var path = Path.Combine(viewPath, fileName);

	        if (!File.Exists(path))
                path = Path.Combine(viewPath, id, fileName);

            if (File.Exists(path))
            {
	            templateInfo = new FileTemplateInfo(id, new FileInfo(path));
	            return true;
	        }

	        return false;
	    }

	    private IEnumerable<TemplateInfo> Read(string directory)
	    {
	        if (!Directory.Exists(directory))
	            return Enumerable.Empty<TemplateInfo>();

	        return Directory.GetFiles(directory, "*.html", SearchOption.AllDirectories).Select(f =>
	        {
	            var info = new FileInfo(f); 
                return new FileTemplateInfo(info.Name, info); 
            });
	    }

	    public IEnumerable<TemplateInfo> GetAll()
	    {
	        return Read(_config.ViewPath)
                .Union(Read(_config.ModulePath));
	    }
	}
}