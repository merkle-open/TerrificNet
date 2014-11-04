using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Mustache;
using TerrificNet.Config;

namespace TerrificNet.Controller
{
	public class TemplateController : ApiController
	{
		private readonly ITerrificNetConfig _config;

		public TemplateController(ITerrificNetConfig config)
		{
			_config = config;
		}

		[Route("template/{*path}")]
		[HttpGet]
		public HttpResponseMessage Get(string path)
		{
			var model = new Dictionary<string, object>();
			model["name"] = "Hans Muster";

			var compiler = new FormatCompiler();

			var filePath = _config.Path + "Templates/" + path;

			if (!File.Exists(filePath))
				return new HttpResponseMessage(HttpStatusCode.NotFound);

			var reader = new StreamReader(filePath);
			var generator = compiler.Compile(reader.ReadToEnd());
			var result = generator.Render(model);

			var message = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(result) };
			message.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
			return message;
		}
	}
}
