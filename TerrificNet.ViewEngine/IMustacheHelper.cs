using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nustache.Core;

namespace TerrificNet.ViewEngine
{
	public interface IMustacheHelper
	{
		void Register(Func<string, bool> contains, Action<string, Helper> register);
	}
}
