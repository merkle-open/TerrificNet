using System.Reflection;

namespace TerrificNet.ViewEngine.Client.Javascript
{
	public class JavascriptClientModel : IClientModel
	{
	    public JavascriptClientModel Parent { get; set; }
	    private readonly string _variableName;

	    public JavascriptClientModel(string variableName) : this(null, variableName)
	    {
	    }

		public JavascriptClientModel(JavascriptClientModel parent, string variableName)
		{
		    Parent = parent;
		    _variableName = variableName;
		}

	    public int Depth
	    {
            get { return Parent != null ? Parent.Depth + 1 : 0; }
	    }

	    public IClientModel Get(string property)
		{
			return new JavascriptClientModel(this, string.Concat(_variableName, ".", property));
		}

		public override string ToString()
		{
			return _variableName;
		}
	}
}