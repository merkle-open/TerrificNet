using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.Client;
using TerrificNet.ViewEngine.Client.Javascript;

namespace TerrificNet.Controllers
{
	public class ClientTemplateController : ApiController
	{
		private readonly ITemplateRepository _templateRepository;

		public ClientTemplateController(ITemplateRepository templateRepository)
		{
			_templateRepository = templateRepository;
		}

		[HttpGet]
		// TODO: Check xss for parameter
		public HttpResponseMessage Get(string path, string jsRepository = "")
		{
			TemplateInfo templateInfo;
			if (!_templateRepository.TryGetTemplate(path, out templateInfo))
				return new HttpResponseMessage(HttpStatusCode.NotFound);

			var generator = new JavascriptClientTemplateGenerator(jsRepository, new ClientTemplateGenerator());
			var message = new HttpResponseMessage
			{
				Content = new StringContent(generator.Generate(templateInfo))
			};

			message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/javascript");

			return message;
		}
	}
}
