using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Practices.Unity;
using TerrificNet.Models;
using TerrificNet.UnityModules;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.ViewEngines.TemplateHandler;

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
		public HttpResponseMessage Index()
		{
			var viewDefinition = DefaultLayout.WithDefaultLayout(new ViewDefinition
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
				Applications = _applications.Select(a => new ViewOverviewModel
				{
					Name = a.Name,
					Modules = a.Container.Resolve<IModuleRepository>().GetAll().Select(m => GetView(a.Section, m)).ToList(),
					Views = GetViews(a.Section, a.Container.Resolve<ITemplateRepository>()).ToList()
				}).ToList()
			};

			return model;
		}

		private static ViewItemModel GetView(string section, ModuleDefinition m)
		{
			var schemaUrl = string.Format("/{0}schema/{1}", section, m.Id);
			var dataUrl = string.Format("/{0}model/{1}", section, m.Id);
			var templateUrl = string.Format("/{0}{1}", section, m.Id);
			var templateId = m.Id;

			return new ViewItemModel
			{
				Text = m.Id,
				Url = templateUrl,
				EditUrl = string.Format("/web/edit?schema={0}&data={1}&template={2}&id={4}&app={3}", schemaUrl, dataUrl, templateUrl, section, templateId),
				AdvancedUrl = string.Format("/web/edit_advanced?schema={0}&data={1}&template={2}&id={4}&app={3}", schemaUrl, dataUrl, templateUrl, section, templateId),
				SchemaUrl = schemaUrl
			};
		}

		private static IEnumerable<ViewItemModel> GetViews(string section, ITemplateRepository templateRepository)
		{
			foreach (var file in templateRepository.GetAll())
			{
				var schemaUrl = string.Format("/{0}schema/{1}", section, file.Id);
				var dataUrl = string.Format("/{0}model/{1}", section, file.Id);
				var templateUrl = string.Format("/{0}{1}", section, file.Id);
				var templateId = file.Id;
				yield return new ViewItemModel
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
