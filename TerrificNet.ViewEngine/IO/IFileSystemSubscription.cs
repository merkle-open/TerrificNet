using System;

namespace TerrificNet.ViewEngine.IO
{
	public interface IFileSystemSubscription : IDisposable
	{
		void Register(Action<string> handler);
	}
}