using System.Collections.Generic;

namespace TerrificNet.ViewEngine.ViewEngines
{
	public interface IRenderingHelperHandlerFactory
	{
		IEnumerable<IRenderingHelperHandler> Create();
	}
}