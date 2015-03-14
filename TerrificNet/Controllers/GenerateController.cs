using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using TerrificNet.Generator;
using TerrificNet.ViewEngine;

namespace TerrificNet.Controllers
{
	public class GenerateController : ApiController
	{
		private readonly IJsonSchemaCodeGenerator _generator;
		private readonly ISchemaProvider _schemaProvider;
	    private readonly ITemplateRepository _templateRepository;

	    public GenerateController(IJsonSchemaCodeGenerator generator, ISchemaProvider schemaProvider, ITemplateRepository templateRepository)
		{
			_generator = generator;
			_schemaProvider = schemaProvider;
	        _templateRepository = templateRepository;
		}

		[Route("generate/{*path}")]
		[HttpGet]
		public async Task<HttpResponseMessage> Get(string path)
		{
		    var templateInfo = await _templateRepository.GetTemplateAsync(path).ConfigureAwait(false);
            if (templateInfo == null)
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Template not found");

			var schema = await _schemaProvider.GetSchemaFromTemplateAsync(templateInfo).ConfigureAwait(false);

            var type = _generator.Compile(schema);
			var code = _generator.Generate(schema);
			var message = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(code) };
			message.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			return message;
		}

	}
}