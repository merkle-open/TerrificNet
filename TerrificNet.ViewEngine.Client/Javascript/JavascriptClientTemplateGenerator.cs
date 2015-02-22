using System.IO;
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
                writer.Write("{0}.register(\"{1}\", {{ render: function(ctx, model) {{ var w = function(v) {{ ctx.write(v); }}; var we = function(v) {{ ctx.writeEscape(v); }};", _templateRepository, templateInfo.Id);
				var clientContext = new JavascriptClientContext(templateInfo.Id, writer);
				var model = new JavascriptClientModel("model");
				_templateGenerator.Generate(templateInfo, clientContext, model);

				writer.Write("}});");	
			}

			return builder.ToString();
		}
	}
}