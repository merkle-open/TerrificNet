using System.IO;
using System.Text;
using Nustache.Core;
using TerrificNet.ViewEngine.Config;

namespace TerrificNet.ViewEngine.ViewEngines
{
	public class NustachePhysicalViewEngine : PhysicalViewEngineBase
	{
		private readonly string _localBasePath;

		public NustachePhysicalViewEngine(ITerrificNetConfig config, IModelProvider modelProvider, ITemplateLocator templateLocator)
		{
			_localBasePath = config.ViewPath;

			new TerrificModuleHelper(this, modelProvider, templateLocator).Register(Helpers.Contains, Helpers.Register);
		}

		protected override IView CreateView(string content)
		{
			var template = new Template();
			template.Load(new StringReader(content));

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
				var path = Path.Combine(_localBasePath, name, ".html");
				if (!File.Exists(path))
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
