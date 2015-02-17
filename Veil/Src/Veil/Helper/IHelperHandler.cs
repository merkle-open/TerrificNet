using System.Collections.Generic;

namespace Veil.Helper
{
	public interface IHelperHandler
	{
		bool IsSupported(string name);
		void Evaluate(object model, string name, IDictionary<string, string> parameters);
	}
}