using System.Collections.Generic;
using TerrificNet.ViewEngine.ViewEngines.TemplateHandler;

namespace TerrificNet.ViewEngine.ViewEngines
{
	public interface ITerrificHelperHandlerFactory
	{
		IEnumerable<ITerrificHelperHandler> Create();
	}
}