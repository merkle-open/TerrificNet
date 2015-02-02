using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using TerrificNet.ViewEngine;

namespace TerrificNet.Controllers
{
    public class SchemaController : ApiController
    {
        private readonly ITemplateRepository _templateRepository;
        private readonly ISchemaProvider _schemaProvider;

        public SchemaController(ITemplateRepository templateRepository, ISchemaProvider schemaProvider)
        {
            _templateRepository = templateRepository;
            _schemaProvider = schemaProvider;
        }

        [HttpGet]
        public HttpResponseMessage Get(string path)
        {
            TemplateInfo templateInfo;
            if (!_templateRepository.TryGetTemplate(path, out templateInfo))
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Template not found");

            var message = new HttpResponseMessage
            {
                Content = new StringContent(_schemaProvider.GetSchemaFromTemplate(templateInfo).ToString())
            };
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return message;
        }
    }
}
