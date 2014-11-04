using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Nustache.Core;

namespace TerrificNet.ViewEngine.ViewEngines
{
	public class NustachePhysicalViewEngine : PhysicalViewEngineBase
	{
		private readonly string _localBasePath;

		public NustachePhysicalViewEngine(string basePath)
			: base(basePath)
		{
			_localBasePath = basePath;
		}

		protected override IView CreateView(string content)
		{
			var template = new Template();
			template.Load(new StringReader(content));

			DisplayHelpers.Register(this);

			return new NustacheView(template, _localBasePath);
		}


		private class NustacheView : IView
		{
			private readonly Template _template;
			private readonly string _localBasePath;

			public NustacheView(Template template, string localBasePath)
			{
				_template = template;
				_localBasePath = localBasePath;
			}

			private Template LoadTemplate(string name)
			{
				var path = _localBasePath + "/" + name + ".html";
				if(!File.Exists(path))
				return null;

				using (var reader = new StreamReader(path))
				{
					var template = new Template(name);
					template.Load(reader);
					return template;
				}
			}

			public string Render(object model)
			{
				var builder = new StringBuilder();
				using (var writer = new StringWriter(builder))
				{
					_template.Render(model, writer, LoadTemplate);
				}

				return builder.ToString();
			}
		}
	}
}
