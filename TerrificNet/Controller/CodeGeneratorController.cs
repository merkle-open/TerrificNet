using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using TerrificNet.Generator;

namespace TerrificNet.Controller
{
	public class CodeGeneratorController : ApiController
	{
		private readonly IJsonSchemaCodeGenerator _generator;

		public CodeGeneratorController(IJsonSchemaCodeGenerator generator)
		{
			_generator = generator;
		}

		[Route("generate")]
		[HttpGet]
		public HttpResponseMessage Get()
		{
			var message = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(_generator.Generate()) };
			message.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
			return message;
		}
	}
}