using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using TerrificNet.ViewEngine;

namespace TerrificNet.Controllers
{
	public class TemplateControllerBase : ApiController
	{
		protected HttpResponseMessage Render(IView view, object model)
		{
			var result = view.Render(model);

			var message = new HttpResponseMessage(HttpStatusCode.OK) {Content = new StringContent(result)};
			message.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
			return message;
		}
	}
}