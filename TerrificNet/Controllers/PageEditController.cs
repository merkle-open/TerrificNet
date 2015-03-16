using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TerrificNet.Models;
using TerrificNet.ViewEngine.TemplateHandler;

namespace TerrificNet.Controllers
{
    public class PageEditController : TemplateControllerBase
    {
        [HttpGet]
        public Task<HttpResponseMessage> Index(string id)
        {
            var viewDefinition = DefaultLayout.WithDefaultLayout(new PartialViewDefinition
            {
                Template = "components/modules/PageEditor/PageEditor",
                Data = new PageEditModel()
                {
                    PageJson = "TUBEL"
                }
            });

            return View(viewDefinition.Template, viewDefinition);
        }

        private PageEditModel GetEditModel()
        {
            return new PageEditModel()
            {
                PageJson = "TEST"
            };
        }
    }
}