using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TerrificNet.ViewEngine.Test
{
	[TestClass]
	public class NamingRuleTest
	{
		private INamingRule _namingRule;

		[TestInitialize]
		public void Init()
		{
			_namingRule = new NamingRule();
		}

		[TestMethod]
		public void TestClassName()
		{
			
		}
	}
}