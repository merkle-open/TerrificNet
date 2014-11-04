using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Nustache.Core;

namespace TerrificNet.ViewEngine
{
	public class NustachePhysicalViewEngine : PhysicalViewEngineBase
	{
		public NustachePhysicalViewEngine(string basePath)
			: base(basePath)
		{
		}

		protected override IView CreateView(string content)
		{
			var template = new Template();
			template.Load(new StringReader(content));

			DisplayHelpers.Register();

			return new NustacheView(template);
		}

		private static Template LoadTemplate(string name)
		{
			return null;
		}

		private class NustacheView : IView
		{
			private readonly Template _template;

			public NustacheView(Template template)
			{
				_template = template;
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
