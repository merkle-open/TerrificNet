namespace TerrificNet.ViewEngine.Client.Javascript
{
	public class JavascriptClientModel : IClientModel
	{
		private readonly string _variableName;

		public JavascriptClientModel(string variableName)
		{
			_variableName = variableName;
		}

		public IClientModel Get(string property)
		{
			return new JavascriptClientModel(string.Concat(_variableName, ".", property));
		}

		public override string ToString()
		{
			return _variableName;
		}
	}
}