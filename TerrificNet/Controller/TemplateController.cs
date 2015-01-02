using System.Net;
using System.Net.Http;
using System.Web.Http;
using TerrificNet.ViewEngine;

namespace TerrificNet.Controller
{
	public class TemplateController : TemplateControllerBase
	{
		private readonly IViewEngine _viewEngine;
		private readonly IModelProvider _modelProvider;
		private readonly ITemplateLocator _templateLocator;

		public TemplateController(IViewEngine viewEngine, IModelProvider modelProvider, ITemplateLocator templateLocator)
		{
			_viewEngine = viewEngine;
			_modelProvider = modelProvider;
			_templateLocator = templateLocator;
		}

		[HttpGet]
		public HttpResponseMessage Get(string path)
		{
			var model = _modelProvider.GetModelFromPath(path);

			IView view;
			string templatePath;
			if (!_templateLocator.TryLocateTemplate(path, out templatePath) ||
				!_viewEngine.TryCreateViewFromPath(templatePath, out view))
				return new HttpResponseMessage(HttpStatusCode.NotFound);

			return Render(view, model);
		}
	}
}
