using Veil.Helper;

namespace TerrificNet.ViewEngine.ViewEngines.TemplateHandler
{
	internal interface ITerrificHelperHandler : IHelperHandler
	{
		void SetContext(RenderingContext context);
	}
}