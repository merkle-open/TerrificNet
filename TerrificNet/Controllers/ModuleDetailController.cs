using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TerrificNet.Models;
using TerrificNet.UnityModules;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.ViewEngines.TemplateHandler;

namespace TerrificNet.Controllers
{
	public class ModuleDetailController : AdministrationTemplateControllerBase
	{
		public ModuleDetailController(TerrificNetApplication[] applications) : base(applications)
		{
		}

		[HttpGet]
		public HttpResponseMessage Index(string id, string app)
		{
			var moduleRepository = this.ResolveForApp<IModuleRepository>(app);
			ModuleDefinition moduleDefinition;
			if (!moduleRepository.TryGetModuleDefinitionById(id, out moduleDefinition))
				return new HttpResponseMessage(HttpStatusCode.NotFound);

			var viewDefinition = DefaultLayout.WithDefaultLayout(new ViewDefinition
			{
				Template = "components/modules/ModuleDetail/ModuleDetail",
				Data = GetOverviewModel(moduleDefinition)
			});

			return View(viewDefinition.Template, viewDefinition);
		}

		private ModuleDetailModel GetOverviewModel(ModuleDefinition moduleDefinition)
		{
			return new ModuleDetailModel
			{
				Name = moduleDefinition.Id,
				DefaultTemplate = GetModel(moduleDefinition.DefaultTemplate),
				Skins = moduleDefinition.Skins.Select(kv => GetModel(kv.Value, kv.Key)).ToList()
			};
		}

		private TemplateItemModel GetModel(TemplateInfo template, string name = null)
		{
			if (template == null)
				return null;

			return new TemplateItemModel
			{
				Text = name ?? template.Id,
				Url = "#"
			};
		}
	}
}
