using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.Client;
using TerrificNet.ViewEngine.Client.Javascript;

namespace TerrificNet.Controllers
{
	public class ClientTemplateController : ApiController
	{
		private readonly ITemplateRepository _templateRepository;
		private readonly IClientTemplateGeneratorFactory _clientFactory;

		public ClientTemplateController(ITemplateRepository templateRepository, IClientTemplateGeneratorFactory clientFactory)
		{
			_templateRepository = templateRepository;
			_clientFactory = clientFactory;
		}

		[HttpGet]
		public async Task<HttpResponseMessage> Get(string path)
		{
		    var templateInfo = await _templateRepository.GetTemplateAsync(path).ConfigureAwait(false);
			if (templateInfo == null)
				return new HttpResponseMessage(HttpStatusCode.NotFound);

			var generator = new JavascriptClientTemplateGenerator("Tcn.TemplateRepository", _clientFactory.Create());
			var message = new HttpResponseMessage
			{
				Content = new StringContent(generator.Generate(templateInfo))
			};

			message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/javascript");

			return message;
		}
	}
}
