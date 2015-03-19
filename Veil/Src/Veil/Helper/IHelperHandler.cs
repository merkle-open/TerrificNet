using System.Collections.Generic;
using System.Threading.Tasks;

namespace Veil.Helper
{
	public interface IHelperHandler
	{
		bool IsSupported(string name);
		Task EvaluateAsync(object model, RenderingContext context, string name, IDictionary<string, string> parameters);
	}
}