using System;
using System.ComponentModel;
using System.IO;
using System.Web;

namespace TerrificNet.ViewEngine.Client.Javascript
{
	public class JavascriptClientContext : IClientContext
	{
		private readonly TextWriter _writer;

		public JavascriptClientContext(string templateId, TextWriter writer)
		{
		    TemplateId = templateId;
		    _writer = writer;
		}

	    public string TemplateId { get; private set; }

	    public void WriteLiteral(string content)
		{
			_writer.Write("w(\"");
			_writer.Write(HttpUtility.JavaScriptStringEncode(content));
			_writer.Write("\");");
		}

		public void WriteExpression(IClientModel model)
		{
            _writer.Write("w(");
			_writer.Write(model.ToString());
			_writer.Write(");");
		}

	    public void WriteEncodeExpression(IClientModel model)
	    {
            _writer.Write("we(");
            _writer.Write(model.ToString());
            _writer.Write(");");
	    }

	    public IClientModel BeginIterate(IClientModel model)
	    {
	        //var itemIdx = model.ToString().Split("item").Length;
	        var jsModel = model as JavascriptClientModel;
			var itemVariable = new JavascriptClientModel(model as JavascriptClientModel, "item" + jsModel.Depth);
            _writer.Write("for (var i{2} = 0; i{2} < {1}.length; i{2}++){{ var {0} = {1}[i{2}]; ", itemVariable, model, jsModel.Depth);

			return itemVariable;
		}

		public void EndIterate()
		{
			_writer.Write("}");
		}

		public void BeginIf(IClientModel model)
		{
			_writer.Write("if ({0}){{", model);
		}

		public void EndIf()
		{
			_writer.Write("}");
		}

		public void ElseIf()
		{
			_writer.Write("} else {");
		}
	}
}