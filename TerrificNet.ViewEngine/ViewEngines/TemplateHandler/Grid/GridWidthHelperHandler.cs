using System.Collections.Generic;

namespace TerrificNet.ViewEngine.ViewEngines.TemplateHandler.Grid
{
	internal class GridWidthHelperHandler : ITerrificHelperHandler
	{
		private RenderingContext _context;

		public bool IsSupported(string name)
		{
			return name.StartsWith("grid-width");
		}

		public void Evaluate(object model, string name, IDictionary<string, string> parameters)
		{
			double ratio = 1.0;
			string ratioValue;
			if (parameters.TryGetValue("ratio", out ratioValue))
			{
				if (!double.TryParse(ratioValue, out ratio))
					ratio = 1.0;
			}

			var gridStack = GridStack.FromContext(_context);
			_context.Writer.Write(gridStack.CurrentWidth * ratio);
		}

		public void SetContext(RenderingContext context)
		{
			_context = context;
		}
	}
}