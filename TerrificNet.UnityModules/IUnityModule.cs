using Microsoft.Practices.Unity;

namespace TerrificNet.UnityModules
{
	public interface IUnityModule
	{
		void Configure(IUnityContainer container);
	}
}