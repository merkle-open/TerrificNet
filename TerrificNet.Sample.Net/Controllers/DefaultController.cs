using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TerrificNet.Sample.Net.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        public ActionResult Index()
        {
            teaserModel.TeaserModel test = new teaserModel.TeaserModel();
            test.Entries = new List<object>();

            teaser_headingModel.Teaser_HeadingModel model = new teaser_headingModel.Teaser_HeadingModel();
            model.Sortings.Add(new teaser_headingModel.Sortings()
            {
                Name ="asdfsdf",
                Key = "asdf"
            });

            return View();
        }
    }
}