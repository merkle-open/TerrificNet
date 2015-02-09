using System.Collections.Generic;

namespace TerrificNet.ViewEngine.ViewEngines.TemplateHandler.Grid
{
	internal class GridWithHelperHandler : ITerrificHelperHandler
	{
		private RenderingContext _context;

		public bool IsSupported(string name)
		{
			return name.StartsWith("grid-width");
		}

		public void Evaluate(object model, string name, IDictionary<string, string> parameters)
		{
			var gridStack = GridStack.FromContext(_context);
			_context.Writer.Write(gridStack.CurrentWidth);
		}

		public void SetContext(RenderingContext context)
		{
			_context = context;
		}
	}
}