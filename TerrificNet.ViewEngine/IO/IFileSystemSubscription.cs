using System;

namespace TerrificNet.ViewEngine.IO
{
	public interface IFileSystemSubscription
	{
		void Register(Action<string> handler);
	}
}