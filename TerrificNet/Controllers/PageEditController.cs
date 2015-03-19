using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TerrificNet.AssetCompiler;
using TerrificNet.AssetCompiler.Helpers;
using TerrificNet.Models;
using TerrificNet.UnityModules;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.Config;
using TerrificNet.ViewEngine.IO;
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
            PageViewDefinition siteDefinition;
            var found = ResolveForApp<TerrificViewDefinitionRepository>(app).TryGetFromViewId(id, out siteDefinition);
            var appViewEnging = ResolveForApp<IViewEngine>(app);
            var tplInfo = await ResolveForApp<ITemplateRepository>(app).GetTemplateAsync(siteDefinition.Template);

            var data = new PageEditModel
            {
                PageJson = found ? JsonConvert.SerializeObject(siteDefinition) : null,
                PageHtml = CreateSiteHtml(await appViewEnging.CreateViewAsync(tplInfo), siteDefinition),
				Modules = new List<PageEditModuleModel>()
                //Modules = await CreateModules(app)
            };

            var viewDefinition = DefaultLayout.WithDefaultLayout(new PartialViewDefinition
            {
                Template = "components/modules/PageEditor/PageEditor",
                Data = data
            });
            viewDefinition.IncludeScript("assets/pageEditor.js");
            viewDefinition.IncludeStyle("assets/pageEditor.css");
            viewDefinition.IncludeStyle("/web/page_edit/bundle_app.css?app=" + app);

            return await View(viewDefinition.Template, viewDefinition);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetEditorAsset(string name, string app = "")
        {
            var assetHelper = ResolveForApp<IAssetHelper>(app);
            var assetBundler = ResolveForApp<IAssetBundler>(app);
            var assetCompilerFactory = ResolveForApp<IAssetCompilerFactory>(app);
            var config = ResolveForApp<ITerrificNetConfig>(app);
            var fileSystem = ResolveForApp<IFileSystem>(app);

            var components = assetHelper.GetGlobComponentsForAsset(config.Assets[name], fileSystem.BasePath);
            var content = await assetBundler.BundleAsync(components);
            content = ".page-editor .page, .page-editor .sidebar{" + content + "}";
            var compiler = assetCompilerFactory.GetCompiler(name);
            var compiledContent = await compiler.CompileAsync(content);

            var response = new HttpResponseMessage
            {
                Content = new StringContent(compiledContent, Encoding.Default, compiler.MimeType)
            };
            return response;
        }

        private static string CreateSiteHtml(IView view, PageViewDefinition siteDefinition)
        {
            var pageHtmlBuilder = new StringBuilder();
            using (var writer = new StringWriter(pageHtmlBuilder))
            {
                var context = new RenderingContext(writer);
                context.Data.Add("pageEditor", true);
	            context.Data.Add("siteDefinition", siteDefinition);

                view.Render(JObject.FromObject(siteDefinition), context);
            }

            var html = pageHtmlBuilder.ToString();

            html = html.Replace("<!DOCTYPE html>", "").Replace("<html>", "").Replace("</html>", "");
            html = new Regex("<head>(.*)</head>", RegexOptions.Singleline).Replace(html, "");
            html = new Regex("<script( src[=]\".*\")?>(.*)</script>", RegexOptions.Singleline).Replace(html, "");

            return html;
        }

        private async Task<IEnumerable<PageEditModuleModel>> CreateModules(string app)
        {
            var siteModules = new List<PageEditModuleModel>();
            var appViewEnging = ResolveForApp<IViewEngine>(app);
            var modelRepository = ResolveForApp<IModuleRepository>(app);
            var modelProvider = ResolveForApp<IModelProvider>(app);
            
            var replacePath = ResolveForApp<ITerrificNetConfig>(app).ModulePath;
            if (!replacePath.EndsWith("/")) replacePath += "/";

            foreach (var mod in modelRepository.GetAll())
            {
                var model = await modelProvider.GetModelForModuleAsync(mod, null);
                var view = await appViewEnging.CreateViewAsync(mod.DefaultTemplate);
                var scriptRx = new Regex("<script( src[=]\".*\")?>(.*)</script>", RegexOptions.Singleline);

                var htmlBuilder = new StringBuilder();
                using (var writer = new StringWriter(htmlBuilder))
                {
                    var context = new RenderingContext(writer);
                    context.Data.Add("pageEditor", true);
                    view.Render(model, context);
                }

                var skins = new List<SkinInfoModel>();
                foreach (var skin in mod.Skins)
                {
                    var skinView = await appViewEnging.CreateViewAsync(skin.Value);
                    var skinBuilder = new StringBuilder();
                    using (var writer = new StringWriter(skinBuilder))
                    {
                        var context = new RenderingContext(writer);
                        context.Data.Add("pageEditor", true);
                        skinView.Render(model, context);
                    }
                    skins.Add(new SkinInfoModel
                    {
                        Name = skin.Key,
                        Html = scriptRx.Replace(skinBuilder.ToString(), "")
                    });
                }

                siteModules.Add(new PageEditModuleModel
                {
                    Id = mod.Id,
                    Name = mod.Id.Replace(replacePath, ""),
                    Html = scriptRx.Replace(htmlBuilder.ToString(), ""),
                    Skins = skins
                });
            }
            return siteModules;
        }
    }
}