using System;
using System.Collections.Generic;
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

			var modelProvider = this.ResolveForApp<IModelProvider>(app);

			var viewDefinition = DefaultLayout.WithDefaultLayout(new PartialViewDefinition
			{
				Template = "components/modules/ModuleDetail/ModuleDetail",
				Data = GetOverviewModel(moduleDefinition, modelProvider.GetDataVariations(moduleDefinition), app)
			});

			viewDefinition.AddAction(new ActionModel
			{
				Name = "Add Data Variation",
				Link = string.Format("/web/module/createdata?id={0}&app={1}", id, app),
			});

			viewDefinition.AddAction(new ActionModel
			{
				Name = "Add Skin",
				Link = "#"
			});

			return View(viewDefinition.Template, viewDefinition);
		}

		[HttpGet]
		public HttpResponseMessage CreateData(string id, string app)
		{
			var moduleRepository = this.ResolveForApp<IModuleRepository>(app);
			ModuleDefinition moduleDefinition;
			if (!moduleRepository.TryGetModuleDefinitionById(id, out moduleDefinition))
				return new HttpResponseMessage(HttpStatusCode.NotFound);

			var dataRepository = this.ResolveForApp<IModelProvider>(app);
			dataRepository.UpdateModelForModule(moduleDefinition, Guid.NewGuid().ToString("N"), null);

			var response = new HttpResponseMessage(HttpStatusCode.Redirect);
			response.Headers.Location = new Uri(string.Format("/web/module?id={0}&app={1}", id, app), UriKind.Relative);

			return response;
		}

		private ModuleDetailModel GetOverviewModel(ModuleDefinition moduleDefinition, IEnumerable<string> dataVariations, string section)
		{
		    var variationsModel = dataVariations.Select(id => GetDataVariation(id, section, moduleDefinition.Id)).ToList();
		    return new ModuleDetailModel
			{
				Name = moduleDefinition.Id,
				DefaultTemplate = GetModel(moduleDefinition.DefaultTemplate),
				Skins = moduleDefinition.Skins.Select(kv => GetModel(kv.Value, kv.Key)).ToList(),
				DataVariations = variationsModel.Count > 0 ? variationsModel : null,
                SchemaUrl = string.Format("/{0}module_schema/{1}", section, moduleDefinition.Id) 
			};
		}

	    private DataVariationModel GetDataVariation(string id, string section, string moduleId)
		{
			return new DataVariationModel
			{
				Name = id,
				Link = string.Format("/{0}model/{1}?dataId={2}", section, moduleId, id),
				DeleteLink = "#",
				EditLink = string.Format("/web/edit?schema=/{0}module_schema/{1}&data=/{0}model/{1}?dataId={2}", section, moduleId, id)
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
