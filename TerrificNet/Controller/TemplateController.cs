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
		private readonly ITemplateRepository _templateRepository;

		public TemplateController(IViewEngine viewEngine, IModelProvider modelProvider, ITemplateRepository templateRepository)
		{
			_viewEngine = viewEngine;
			_modelProvider = modelProvider;
			_templateRepository = templateRepository;
		}

		[HttpGet]
		public HttpResponseMessage Get(string path)
		{
			var model = _modelProvider.GetModelForTemplate(path);

			IView view;
			TemplateInfo templateInfo;
			if (!_templateRepository.TryGetTemplate(path, out templateInfo) ||
				!_viewEngine.TryCreateView(templateInfo, out view))
				return new HttpResponseMessage(HttpStatusCode.NotFound);

			return Render(view, model);
		}
	}
}
