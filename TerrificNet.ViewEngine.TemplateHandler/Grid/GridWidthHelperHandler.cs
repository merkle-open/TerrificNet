using System.Collections.Generic;
using System.Threading.Tasks;
using Veil;
using Veil.Helper;

namespace TerrificNet.ViewEngine.TemplateHandler.Grid
{
	internal abstract class BaseGridWidthHelperHandler : IHelperHandler
	{
	    public abstract bool IsSupported(string name);

		internal abstract double GetWidth(GridStack gridStack);

		public Task EvaluateAsync(object model, RenderingContext context, IDictionary<string, string> parameters)
		{
			double ratio = 1.0;
			string ratioValue;
			if (parameters.TryGetValue("ratio", out ratioValue))
			{
				if (!double.TryParse(ratioValue, out ratio))
					ratio = 1.0;
			}

			var gridStack = GridStack.FromContext(context);
		    var value = (int)(GetWidth(gridStack) * ratio);
		    return context.Writer.WriteAsync(value.ToString());
		}
	}

	internal class GridWidthHelperHandler : BaseGridWidthHelperHandler
	{
		public override bool IsSupported(string name)
		{
			return name.StartsWith("grid-width");
		}

		internal override double GetWidth(GridStack gridStack)
		{
			return gridStack.Current.Width;
		}
	}

	internal class GridComponentWidthHelperHandler : BaseGridWidthHelperHandler
	{
		public override bool IsSupported(string name)
		{
			return name.StartsWith("grid-component-width");
		}

		internal override double GetWidth(GridStack gridStack)
		{
			return gridStack.Current.Width - gridStack.Current.ComponentPadding * 2;
		}
	}
}