using System.Collections.Generic;
using Nustache.Core;

namespace TerrificNet.ViewEngine
{
	public class DisplayHelpers
	{
		public static void Register()
		{
			if (!Helpers.Contains("module"))
				Helpers.Register("module", TerrificModuleHelper);
		}

		internal static void TerrificModuleHelper(RenderContext ctx, IList<object> args, IDictionary<string, object> options,
			RenderBlock fn, RenderBlock inverse)
		{
			object template;
			if (!options.TryGetValue("template", out template))
				return;

			var templateName = template as string;
			if (string.IsNullOrEmpty(templateName))
				return;

			ctx.Write("start("+templateName+")");
			ctx.Include(templateName, "");
		}
	}
}