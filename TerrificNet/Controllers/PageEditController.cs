using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using TerrificNet.Models;
using TerrificNet.UnityModules;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.TemplateHandler;
using Veil;

namespace TerrificNet.Controllers
{
    public class PageEditController : AdministrationTemplateControllerBase
    {
        public PageEditController(TerrificNetApplication[] applications)
            : base(applications)
        {
        }

        [HttpGet]
        public async Task<HttpResponseMessage> Index(string id, string app)
        {
            PageViewDefinition siteDefinition = null;
            var found = ResolveForApp<TerrificViewDefinitionRepository>(app).TryGetFromViewId(id, out siteDefinition);
            var modules = ResolveForApp<IModuleRepository>(app).GetAll();

            var tplInfo = await ResolveForApp<ITemplateRepository>(app).GetTemplateAsync(siteDefinition.Template);
            var appViewEnging = ResolveForApp<IViewEngine>(app);



            var aa = await appViewEnging.CreateViewAsync(tplInfo);

            

            using (var fudi = new StreamWriter(new MemoryStream()))
            {
                var blub = new RenderingContext(fudi);
                blub.Data.Add("something", "futz");

                await aa.RenderAsync(siteDefinition, blub);

            }


            
            var viewDefinition = DefaultLayout.WithDefaultLayout(new PartialViewDefinition
            {
                Template = "components/modules/PageEditor/PageEditor",
                Data = new PageEditModel
                {
                    PageJson = found ? JsonConvert.SerializeObject(siteDefinition) : null
                }
            });
            viewDefinition.IncludeScript("assets/pageEditor.js");

            return await View(viewDefinition.Template, viewDefinition);
        }
    }
}