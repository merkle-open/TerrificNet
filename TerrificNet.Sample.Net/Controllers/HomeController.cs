using System.Web.Mvc;
using TerrificNet.Mvc;

namespace TerrificNet.Sample.Net.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View("views/content_home");
        }
    }
}