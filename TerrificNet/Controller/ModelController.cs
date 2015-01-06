using System.Web.Http;
using TerrificNet.ViewEngine;

namespace TerrificNet.Controller
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

	        return Json(_modelProvider.GetModelForTemplate(templateInfo));
		}

        [HttpPut]
	    public void Put(string path, [FromBody] object content)
        {
            TemplateInfo templateInfo;
            if (_templateRepository.TryGetTemplate(path, string.Empty, out templateInfo))
                _modelProvider.UpdateModelForTemplate(templateInfo, content);
        }
	}
}
