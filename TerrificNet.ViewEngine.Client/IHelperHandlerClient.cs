using System.Collections.Generic;

namespace TerrificNet.ViewEngine.Client
{
	public interface IHelperHandlerClient
	{
		IClientModel Evaluate(IClientContext context, IClientModel peek, string name, IDictionary<string, string> parameters);
	}

	public interface IBlockHelperHandlerClient : IHelperHandlerClient
	{
		void Leave(IClientContext context, IClientModel peek, string name, IDictionary<string, string> parameters);
	}
}