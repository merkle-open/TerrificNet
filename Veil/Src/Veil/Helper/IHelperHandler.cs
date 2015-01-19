using System.Collections;
using System.Collections.Generic;

namespace Veil.Helper
{
	public interface IHelperHandler
	{
		void Evaluate(object model, string name, IDictionary<string, string> parameters);
	}
}