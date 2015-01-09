using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using navmetaModel;

namespace TerrificNet.Sample.Net.Controllers
{
    public class NavMetaController : Controller
    {
        // GET: NavMeta
        public ActionResult Index()
        {
            var model = new NavmetaModel
            {
                Items = new List<Item>
                {
                    new Item { Name = "1", Href = "asdf", Title = "test"},
                    new Item { Name = "2", Href = "asdf2", Title = "test"},
                }
            };

            return View(model);
        }
    }
}