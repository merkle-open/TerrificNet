using System.Collections.Generic;

namespace TerrificNet.ViewEngine
{
	public interface ITemplateRepository
	{
		bool TryGetTemplate(string id, out TemplateInfo template);

	    IEnumerable<TemplateInfo> GetAll();
	}
}