using System.Collections.Generic;
using Veil.Helper;

namespace TerrificNet.ViewEngine.ViewEngines.TemplateHandler
{
	public interface ITerrificHelperHandler : IHelperHandler
	{
		void PushContext(RenderingContext context);
		void PopContext();
	}
}