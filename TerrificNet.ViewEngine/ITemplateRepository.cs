using System.Collections.Generic;

namespace TerrificNet.ViewEngine
{
	public interface ITemplateRepository
	{
		bool TryGetTemplate(string id, string skin, out TemplateInfo template);

	    IEnumerable<TemplateInfo> GetAll();
	}
}