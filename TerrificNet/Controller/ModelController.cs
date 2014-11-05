using System.Web.Http;
using TerrificNet.ViewEngine;

namespace TerrificNet.Controller
{
	public class ModelController : ApiController
	{
	    private readonly IModelProvider _modelProvider;

        public ModelController(IModelProvider modelProvider)
	    {
	        _modelProvider = modelProvider;
	    }

		[HttpGet]
		public object Get(string path)
		{
	        return Json(_modelProvider.GetModelFromPath(path));
		}

        [HttpPut]
	    public void Put(string path, [FromBody] object content)
        {
            _modelProvider.UpdateModelFromPath(path, content);
        }
	}
}
