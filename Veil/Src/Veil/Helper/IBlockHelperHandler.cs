using System.Collections.Generic;

namespace Veil.Helper
{
	public interface IBlockHelperHandler : IHelperHandler
	{
		void Leave(object model, string name, IDictionary<string, string> parameters);
	}
}