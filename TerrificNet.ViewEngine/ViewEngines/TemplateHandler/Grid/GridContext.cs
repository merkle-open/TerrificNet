namespace TerrificNet.ViewEngine.ViewEngines.TemplateHandler.Grid
{
	public class GridContext
	{
		public GridContext Parent { get; private set; }
		public double Width { get; private set; }

		internal GridContext(GridContext parent, double width)
		{
			Parent = parent;
			Width = width;
		}

		public static GridContext GetFromRenderingContext(RenderingContext renderingContext)
		{
			return GridStack.FromContext(renderingContext).Current;
		}
	}
}