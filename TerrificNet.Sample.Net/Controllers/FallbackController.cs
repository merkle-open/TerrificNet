using System.Web.Mvc;
using TerrificNet.Mvc;
using TerrificNet.ViewEngine.TemplateHandler.Grid;

namespace TerrificNet.Sample.Net.Controllers
{
    public class FallbackController : Controller
    {
        public string ViewName { get; set; }

        // GET: Fallback
        public ActionResult Index()
        {
	        var context = MvcRenderingContext.GetFromViewContext(this.ControllerContext.ParentActionViewContext);
	        var gridContext = GridContext.GetFromRenderingContext(context);

            return View(ViewName, null);
        }
    }
}