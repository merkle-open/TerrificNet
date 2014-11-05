using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using TerrificNet.Generator;
using TerrificNet.ViewEngine;

namespace TerrificNet.Controller
{
	public class GenerateController : ApiController
	{
		private readonly IJsonSchemaCodeGenerator _generator;
		private readonly ISchemaProvider _schemaProvider;

		public GenerateController(IJsonSchemaCodeGenerator generator, ISchemaProvider schemaProvider)
		{
			_generator = generator;
			_schemaProvider = schemaProvider;
		}

		[Route("generate/{*path}")]
		[HttpGet]
		public HttpResponseMessage Get(string path)
		{
			var schema = _schemaProvider.GetSchemaFromPath(path);

			var message = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(_generator.Generate(schema)) };
			message.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
			return message;
		}
	}
}