using System.IO;
using System.Text;
using Nustache.Core;

namespace TerrificNet.ViewEngine.ViewEngines
{
	public class NustachePhysicalViewEngine : IViewEngine
	{
	    private readonly ITemplateRepository _templateRepository;

	    public NustachePhysicalViewEngine(IModelProvider modelProvider, ITemplateRepository templateRepository)
		{
		    _templateRepository = templateRepository;

		    new TerrificModuleHelper(this, modelProvider, _templateRepository).Register(Helpers.Contains, Helpers.Register);
		}

	    public IView CreateView(string content)
		{
			var template = new Template();
			template.Load(new StringReader(content));

            return new NustacheView(template, _templateRepository);
		}

		private class NustacheView : IView
		{
			private readonly Template _template;
		    private readonly ITemplateRepository _templateRepository;

			public NustacheView(Template template, ITemplateRepository templateRepository)
			{
				_template = template;
			    _templateRepository = templateRepository;
			}

			private Template LoadTemplate(string name)
			{
			    TemplateInfo templateInfo;
                if (!_templateRepository.TryGetTemplate(name, out templateInfo))
                    return null;

				using (var reader = new StreamReader(templateInfo.Open()))
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

	    public bool TryCreateView(TemplateInfo templateInfo, out IView view)
	    {
	        view = null;
	        if (templateInfo == null)
	            return false;

	        using (var reader = new StreamReader(templateInfo.Open()))
	        {
	            var content = reader.ReadToEnd();

	            view = CreateView(content);
	            return true;
	        }
	    }
	}
}
