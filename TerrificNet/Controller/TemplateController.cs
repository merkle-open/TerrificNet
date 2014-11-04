using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using TerrificNet.ViewEngine;

namespace TerrificNet.Controller
{
	public class TemplateController : ApiController
	{
	    private readonly IViewEngine _viewEngine;
	    private readonly IModelProvider _modelProvider;

	    public TemplateController(IViewEngine viewEngine, IModelProvider modelProvider)
	    {
	        _viewEngine = viewEngine;
	        _modelProvider = modelProvider;
	    }

	    [Route("template/{*path}")]
		[HttpGet]
		public HttpResponseMessage Get(string path)
		{
	        var model = _modelProvider.GetModelFromPath(path);

	        IView view;
            if (!_viewEngine.TryCreateViewFromPath(path, out view))
                return new HttpResponseMessage(HttpStatusCode.NotFound);

	        var result = view.Render(model);

			var message = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(result)};
			message.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
			return message;
		}
	}
}
