using Veil.Helper;

namespace TerrificNet.ViewEngine.ViewEngines
{
	public interface IRenderingHelperHandler : IHelperHandler
	{
		void PushContext(RenderingContext context);
		void PopContext();
	}
}