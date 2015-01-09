using System;
using System.Web.Mvc;
using TerrificNet.Sample.Net.Models.FooterAddressModel;

namespace TerrificNet.Sample.Net.Controllers
{
    public class FooterAddressController : Controller
    {
        // GET: Default
        public ActionResult Index()
        {
            var address = new FooterAddressModel
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