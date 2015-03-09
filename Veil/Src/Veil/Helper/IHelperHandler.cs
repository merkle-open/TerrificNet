using System.Collections.Generic;

namespace Veil.Helper
{
	public interface IHelperHandler
	{
		bool IsSupported(string name);
		void Evaluate(object model, RenderingContext context, string name, IDictionary<string, string> parameters);
	}
}