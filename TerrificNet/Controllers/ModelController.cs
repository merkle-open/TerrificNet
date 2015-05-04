using System.Threading.Tasks;
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
		public async Task<object> Get(string path, string dataId = null)
	    {
	        var moduleDefinition = await _moduleRepository.GetModuleDefinitionByIdAsync(path).ConfigureAwait(false);
	        if (moduleDefinition == null)
	            return this.NotFound();

	        return Json(await _modelProvider.GetModelForModuleAsync(moduleDefinition, dataId).ConfigureAwait(false));
	    }

        [HttpPut]
		public async Task Put(string path, [FromBody] object content, string dataId = null)
        {
            var moduleDefinition = await _moduleRepository.GetModuleDefinitionByIdAsync(path).ConfigureAwait(false);
            if (moduleDefinition != null)
                await _modelProvider.UpdateModelForModuleAsync(moduleDefinition, dataId, content).ConfigureAwait(false);
        }
	}
}
