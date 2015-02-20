using System.Collections.Generic;

namespace TerrificNet.ViewEngine.Client
{
	public interface IHelperHandlerClient
	{
		void Evaluate(string name, IDictionary<string, string> parameters);
	}
}