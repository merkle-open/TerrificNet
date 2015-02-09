using System.Collections.Generic;

namespace TerrificNet.ViewEngine.ViewEngines.TemplateHandler.Grid
{
	internal class GridStack
	{
		private const string Gridstackkey = "gridstack";
		private readonly Stack<double> _with = new Stack<double>();
		private readonly int _fullWidth;

		private GridStack()
		{
			_fullWidth = 1024;
		}

		public void Push(double width)
		{
			_with.Push(width);
		}

		public double CurrentWidth
		{
			get { return _with.Count > 0 ? _with.Peek() : _fullWidth; }
		}

		public void Pop()
		{
			_with.Pop();
		}

		public static GridStack FromContext(RenderingContext renderingContext)
		{
			object stack;
			GridStack gridStack;
			if (!renderingContext.Data.TryGetValue(Gridstackkey, out stack))
			{
				gridStack = new GridStack();
				renderingContext.Data.Add(Gridstackkey, gridStack);
			}
			else
			{
				gridStack = stack as GridStack;
				if (gridStack == null)
				{
					gridStack = new GridStack();
					renderingContext.Data[Gridstackkey] = gridStack;
				}
			}
			return gridStack;
		}
	}
}