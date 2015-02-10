using System.Collections.Generic;
using Veil.Helper;

namespace TerrificNet.ViewEngine.ViewEngines.TemplateHandler
{
	internal interface ITerrificHelperHandler : IHelperHandler
	{
		void PushContext(RenderingContext context);
		void PopContext();
	}
}