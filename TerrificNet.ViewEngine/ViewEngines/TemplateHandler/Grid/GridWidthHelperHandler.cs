using System.Collections.Generic;

namespace TerrificNet.ViewEngine.ViewEngines.TemplateHandler.Grid
{
	internal class GridWidthHelperHandler : ITerrificHelperHandler
	{
		private readonly Stack<RenderingContext> _contextStack = new Stack<RenderingContext>();

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

			var gridStack = GridStack.FromContext(Context);
			Context.Writer.Write((int)(gridStack.Current.Width * ratio));
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
}