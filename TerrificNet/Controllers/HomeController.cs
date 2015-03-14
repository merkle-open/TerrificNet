using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Practices.Unity;
using TerrificNet.Models;
using TerrificNet.UnityModules;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.TemplateHandler;

namespace TerrificNet.Controllers
{
    public class HomeController : TemplateControllerBase
    {
        private readonly TerrificNetApplication[] _applications;

        public HomeController(TerrificNetApplication[] applications)
        {
            _applications = applications;
        }

        [HttpGet]
        public Task<HttpResponseMessage> Index()
        {
            var viewDefinition = DefaultLayout.WithDefaultLayout(new PartialViewDefinition
            {
                Template = "components/modules/ApplicationOverview/ApplicationOverview",
                Data = GetOverviewModel()
            });

            return View(viewDefinition.Template, viewDefinition);
        }

        private ApplicationOverviewModel GetOverviewModel()
        {
            var model = new ApplicationOverviewModel
            {
                Applications = _applications.Select(a =>
                {
                    var moduleDefinitions = a.Container.Resolve<IModuleRepository>().GetAll().ToList();
                    var templatesInModules =
                        moduleDefinitions.SelectMany(f => f.Skins.Values)
                            .Union(moduleDefinitions.Select(m => m.DefaultTemplate));

                    var partials = a.Container.Resolve<ITemplateRepository>().GetAll().Except(templatesInModules);

                    return new ViewOverviewModel
                    {
                        Name = a.Name,
                        Modules = moduleDefinitions.Select(m => GetView(a.Section, m)).ToList(),
                        Views = a.Container.Resolve<TerrificViewDefinitionRepository>().GetAll().Select(m => GetViews(a.Section, m)).ToList(),
                        Partials = partials.Select(m => GetView(a.Section, m)).ToList()
                    };
                }).ToList()
            };

            return model;
        }

        private static TemplateItemModel GetViews(string section, PageViewDefinition pageViewDefinition)
        {
            var schemaUrl = "/web/viewdefinition_schema.json";
            var dataUrl = string.Format("/{0}model/{1}", section, pageViewDefinition.Id);
            var templateUrl = string.Format("/{0}{1}", section, pageViewDefinition.Id);

            return new TemplateItemModel
            {
                Text = pageViewDefinition.Id,
                Url = string.Format("/web/module?id={0}&app={1}", pageViewDefinition.Id, section),
                EditUrl = string.Format("/web/edit?schema={0}&data={1}&template={2}&id={4}&app={3}", schemaUrl, dataUrl, templateUrl, section, pageViewDefinition.Id),
            };
        }

        private static TemplateItemModel GetView(string section, ModuleDefinition m)
        {
            var schemaUrl = string.Format("/{0}schema/{1}", section, m.Id);
            var dataUrl = string.Format("/{0}model/{1}", section, m.Id);
            var templateUrl = string.Format("/{0}{1}", section, m.Id);
            var templateId = m.Id;

            return new TemplateItemModel
            {
                Text = m.Id,
                Url = string.Format("/web/module?id={0}&app={1}", m.Id, section),
                EditUrl = string.Format("/web/edit?schema={0}&data={1}&template={2}&id={4}&app={3}", schemaUrl, dataUrl, templateUrl, section, templateId),
                AdvancedUrl = string.Format("/web/edit_advanced?schema={0}&data={1}&template={2}&id={4}&app={3}", schemaUrl, dataUrl, templateUrl, section, templateId),
                SchemaUrl = schemaUrl
            };
        }

        private static TemplateItemModel GetView(string section, TemplateInfo m)
        {
            var schemaUrl = string.Format("/{0}schema/{1}", section, m.Id);
            var templateUrl = string.Format("/{0}{1}", section, m.Id);
            var templateId = m.Id;

            return new TemplateItemModel
            {
                Text = m.Id,
                Url = string.Format("/web/module?id={0}&app={1}", m.Id, section),
                SchemaUrl = schemaUrl
            };
        }


        private static IEnumerable<TemplateItemModel> GetViews(string section, ITemplateRepository templateRepository)
        {
            foreach (var file in templateRepository.GetAll())
            {
                var schemaUrl = string.Format("/{0}schema/{1}", section, file.Id);
                var dataUrl = string.Format("/{0}model/{1}", section, file.Id);
                var templateUrl = string.Format("/{0}{1}", section, file.Id);
                var templateId = file.Id;
                yield return new TemplateItemModel
                {
                    Text = file.Id,
                    Url = templateUrl,
                    EditUrl = string.Format("/web/edit?schema={0}&data={1}&template={2}&id={4}&app={3}", schemaUrl, dataUrl, templateUrl, section, templateId),
                    AdvancedUrl = string.Format("/web/edit_advanced?schema={0}&data={1}&template={2}&id={4}&app={3}", schemaUrl, dataUrl, templateUrl, section, templateId),
                    SchemaUrl = schemaUrl
                };
            }
        }
    }
}
