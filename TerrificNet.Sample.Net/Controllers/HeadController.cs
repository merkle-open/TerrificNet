using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TerrificNet.Sample.Net.Models.HeadModel;

namespace TerrificNet.Sample.Net.Controllers
{
    public class HeadController : Controller
    {
        // GET: Head
        public ActionResult Index()
        {
            return View(new HeadModel());
        }
    }
}