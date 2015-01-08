using System.Web.Mvc;
using Newtonsoft.Json.Linq;

namespace TerrificNet.Sample.Net.Controllers
{
    public class FallbackController : Controller
    {
        public string ViewName { get; set; }

        // GET: Fallback
        public ActionResult Index()
        {
            return View(ViewName, new JObject());
        }
    }
}