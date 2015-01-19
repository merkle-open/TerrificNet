using System.IO;
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
	        using (var writer = new StringWriter())
	        {
	            view.Render(model, new RenderingContext(writer));

	            var message = new HttpResponseMessage(HttpStatusCode.OK) {Content = new StringContent(writer.ToString())};
	            message.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
	            return message;
	        }
		}
	}
}