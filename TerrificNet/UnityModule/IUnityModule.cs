using Microsoft.Practices.Unity;

namespace TerrificNet.UnityModule
{
	public interface IUnityModule
	{
		void Configure(IUnityContainer container);
	}
}