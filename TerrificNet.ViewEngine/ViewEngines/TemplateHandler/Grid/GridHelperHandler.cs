using System.Collections.Generic;
using Veil.Helper;

namespace TerrificNet.ViewEngine.ViewEngines.TemplateHandler.Grid
{
	internal class GridHelperHandler : IBlockHelperHandler, ITerrificHelperHandler
	{
		private RenderingContext _context;

		private static readonly Dictionary<string, double> DefaultRatioTable = new Dictionary<string, double>
		{
			{"1/4", 0.25},
			{"1/2", 0.5},
			{"3/4", 0.75},
			{"1/3", 0.33},
			{"2/3", 0.66}
		};

		public bool IsSupported(string name)
		{
			return name.StartsWith("grid-cell");
		}

		public void Evaluate(object model, string name, IDictionary<string, string> parameters)
		{
			var gridStack = GridStack.FromContext(_context);
			string value;
			if (parameters.TryGetValue("ratio", out value))
			{
				double ratio;
				if (!double.TryParse(value, out ratio) && !DefaultRatioTable.TryGetValue(value, out ratio))
				{
					gridStack.Push(gridStack.CurrentWidth);
				}
				else
				{
					gridStack.Push(gridStack.CurrentWidth * ratio);
				}
			}
			else
				gridStack.Push(gridStack.CurrentWidth);
		}

		public void Leave(object model, string name, IDictionary<string, string> parameters)
		{
			var gridStack = GridStack.FromContext(_context);
			gridStack.Pop();
		}

		public void SetContext(RenderingContext context)
		{
			_context = context;
		}
	}
}