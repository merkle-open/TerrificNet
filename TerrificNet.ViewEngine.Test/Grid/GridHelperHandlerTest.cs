using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerrificNet.ViewEngine.ViewEngines.TemplateHandler.Grid;

namespace TerrificNet.ViewEngine.Test.Grid
{
	[TestClass]
	public class GridHelperHandlerTest
	{
		[TestMethod]
		public void TestCalculation1_3()
		{
			var underTest = new GridHelperHandler();

			var renderingContext = new RenderingContext(null);
			underTest.PushContext(renderingContext);
			var gridStack = GridStack.FromContext(renderingContext);
			gridStack.Push(960);

			underTest.Evaluate(null, "", new Dictionary<string, string>
			{
				{"ratio", "1/3"}
			});

			var result = gridStack.Current;
			Assert.AreEqual(320, result.Width);
		}
	}
}
