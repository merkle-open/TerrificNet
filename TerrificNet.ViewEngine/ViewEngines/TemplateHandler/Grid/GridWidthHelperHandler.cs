using System.Collections.Generic;

namespace TerrificNet.ViewEngine.ViewEngines.TemplateHandler.Grid
{
	internal abstract class BaseGridWidthHelperHandler : ITerrificHelperHandler
	{
		private readonly Stack<RenderingContext> _contextStack = new Stack<RenderingContext>();

		public abstract bool IsSupported(string name);

		internal abstract double GetWidth(GridStack gridStack);

		public void Evaluate(object model, string name, IDictionary<string, string> parameters)
		{
			double ratio = 1.0;
			string ratioValue;
			if (parameters.TryGetValue("ratio", out ratioValue))
			{
				if (!double.TryParse(ratioValue, out ratio))
					ratio = 1.0;
			}

			var gridStack = GridStack.FromContext(Context);
			Context.Writer.Write((int)(GetWidth(gridStack) * ratio));
		}

		private RenderingContext Context { get { return _contextStack.Peek(); } }

		public void PushContext(RenderingContext context)
		{
			_contextStack.Push(context);
		}

		public void PopContext()
		{
			_contextStack.Pop();
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