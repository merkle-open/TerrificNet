using System.IO;
using TerrificNet.ViewEngine.Config;

namespace TerrificNet.ViewEngine
{
	public class TerrificTemplateLocator : ITemplateLocator
	{
		private const string HtmlExtension = "html";

		private readonly ITerrificNetConfig _config;

		public TerrificTemplateLocator(ITerrificNetConfig config)
		{
			_config = config;
		}

		public bool TryLocateTemplate(string name, out string path)
		{
			var fileName = Path.ChangeExtension(name, HtmlExtension);

			// locate views
			path = Path.Combine(_config.ViewPath, fileName);

			if (File.Exists(path))
				return true;

			path = Path.Combine(_config.ModulePath, name, fileName);

			return File.Exists(path);
		}
	}
}