using System.Collections.Generic;
using System.IO;
using TerrificNet.ViewEngine.Config;
using TerrificNet.ViewEngine.ViewEngines;

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
			var path = Path.Combine(_config.ViewPath, fileName);

		    if (File.Exists(path))
		    {
		        templateInfo = new FileTemplateInfo(id, new FileInfo(path));
		        return true;
		    }

		    path = Path.Combine(_config.ModulePath, id, fileName);
		    if (File.Exists(path))
		    {
		        templateInfo = new FileTemplateInfo(id, new FileInfo(path));
                return true;
		    }
		    return false;
		}

	    public IEnumerable<TemplateInfo> GetAll()
	    {
	        throw new System.NotImplementedException();
	    }
	}
}