using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using footer_addressModel;

namespace TerrificNet.Sample.Net.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        public ActionResult Index()
        {
            //var test = new teaserModel.TeaserModel();
            //test.Entries = new List<object>();

            //var model = new teaser_headingModel.Teaser_HeadingModel();
            //model.Sortings.Add(new teaser_headingModel.Sortings()
            //{
            //    Name ="asdfsdf",
            //    Key = "asdf",
            //});

            //model.OrderBy = "safd";

            //var entry = new teaser_entryModel.Teaser_EntryModel();

            var address = new Footer_AddressModel
            {
                Address = new Address()
                {
                    Location = "Bern",
                    Street = "Strasse",
                    ZipCode = "9999"
                },
                Mail = "info@test.com",
                Title = "Title" + DateTime.Now.ToLongTimeString()
            };

            return View("footer_address", address);
        }
    }
}