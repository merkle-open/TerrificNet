using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TerrificNet.ViewEngine.Client.Javascript
{
	public class JavascriptClientTemplateGenerator
	{
		private readonly string _templateRepository;
		private readonly IClientTemplateGenerator _templateGenerator;

		public JavascriptClientTemplateGenerator(string templateRepository, IClientTemplateGenerator templateGenerator)
		{
			_templateRepository = templateRepository;
			_templateGenerator = templateGenerator;
		}

		public string Generate(TemplateInfo templateInfo)
		{
			var builder = new StringBuilder();
			using (var writer = new StringWriter(builder))
			{
				//var ns = GetFunctionQualifier(_templateRepository);
                //var nsBuilder = new StringBuilder();
                //writer.Write("var ");
                //foreach (var part in ns)
                //{
                //    nsBuilder.Append(part);
                //    writer.WriteLine("{0} = {0} || {{}};", nsBuilder);
                //    nsBuilder.Append('.');
                //}

                //nsBuilder.Remove(nsBuilder.Length - 1, 1);
                writer.Write("{0}.register(\"{1}\", {{ render: function(model) {{ var out = \"\";", _templateRepository, templateInfo.Id);
				var clientContext = new JavascriptClientContext(templateInfo.Id, writer);
				var model = new JavascriptClientModel("model");
				_templateGenerator.Generate(templateInfo, clientContext, model);

				writer.Write("return out; }});");	
			}

			return builder.ToString();
		}

		private IEnumerable<string> GetFunctionQualifier(string typename)
		{
			return typename.Split('.');
		}
	}
}