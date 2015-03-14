using System.Collections.Generic;
using System.Threading.Tasks;

namespace TerrificNet.ViewEngine
{
	public interface ITemplateRepository
	{
		Task<TemplateInfo> GetTemplateAsync(string id);

	    IEnumerable<TemplateInfo> GetAll();
	}
}