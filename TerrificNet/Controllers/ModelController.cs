using System.Web.Http;
using TerrificNet.ViewEngine;

namespace TerrificNet.Controllers
{
	public class ModelController : ApiController
	{
	    private readonly IModelProvider _modelProvider;
	    private readonly ITemplateRepository _templateRepository;

	    public ModelController(IModelProvider modelProvider, ITemplateRepository templateRepository)
        {
            _modelProvider = modelProvider;
            _templateRepository = templateRepository;
        }

	    [HttpGet]
		public object Get(string path)
	    {
	        TemplateInfo templateInfo;
	        if (!_templateRepository.TryGetTemplate(path, string.Empty, out templateInfo))
	            return this.NotFound();

	        return Json(_modelProvider.GetDefaultModelForTemplate(templateInfo));
		}

        [HttpPut]
	    public void Put(string path, [FromBody] object content)
        {
            TemplateInfo templateInfo;
            if (_templateRepository.TryGetTemplate(path, string.Empty, out templateInfo))
                _modelProvider.UpdateDefaultModelForTemplate(templateInfo, content);
        }
	}
}
