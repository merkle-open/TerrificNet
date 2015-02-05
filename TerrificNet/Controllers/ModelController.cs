using System.Web.Http;
using TerrificNet.ViewEngine;

namespace TerrificNet.Controllers
{
	public class ModelController : ApiController
	{
	    private readonly IModelProvider _modelProvider;
		private readonly IModuleRepository _moduleRepository;

	    public ModelController(IModelProvider modelProvider, IModuleRepository moduleRepository)
        {
            _modelProvider = modelProvider;
            _moduleRepository = moduleRepository;
        }

	    [HttpGet]
		public object Get(string path, string dataId = null)
	    {
	        ModuleDefinition moduleDefinition;
	        if (!_moduleRepository.TryGetModuleDefinitionById(path, out moduleDefinition))
	            return this.NotFound();

	        return Json(_modelProvider.GetModelForModule(moduleDefinition, dataId));
	    }

        [HttpPut]
		public void Put(string path, [FromBody] object content, string dataId = null)
        {
            ModuleDefinition moduleDefinition;
            if (_moduleRepository.TryGetModuleDefinitionById(path, out moduleDefinition))
                _modelProvider.UpdateModelForModule(moduleDefinition, dataId, content);
        }
	}
}
