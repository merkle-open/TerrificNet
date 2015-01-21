using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.Config;

namespace TerrificNet.Controllers
{
	public class TemplateController : TemplateControllerBase
	{
		private readonly IViewEngine _viewEngine;
		private readonly IModelProvider _modelProvider;
		private readonly ITemplateRepository _templateRepository;
	    private readonly ITerrificNetConfig _configuration;

	    public TemplateController(IViewEngine viewEngine, IModelProvider modelProvider, ITemplateRepository templateRepository,
            ITerrificNetConfig configuration)
		{
			_viewEngine = viewEngine;
			_modelProvider = modelProvider;
			_templateRepository = templateRepository;
		    _configuration = configuration;
		}

		[HttpGet]
		public async Task<HttpResponseMessage> Get(string path, string skin = null, string data = null)
		{
		    path = path ?? "index";

			IView view;
			TemplateInfo templateInfo;
		    if (!_templateRepository.TryGetTemplate(path, skin, out templateInfo) ||
		        !_viewEngine.TryCreateView(templateInfo, out view))
		    {
		        var fileName = Path.ChangeExtension(Path.Combine(_configuration.ViewPath, path), "html.json");
		        if (File.Exists(fileName))
		        {
                    using (var reader = new JsonTextReader(new StreamReader(fileName)))
                    {
                        var viewDefinition = new JsonSerializer().Deserialize<ViewDefinition>(reader);

                        if (viewDefinition != null)
                        {
                            using (var client = new HttpClient())
                            {
                                client.BaseAddress = this.Request.RequestUri;
                                var result = await client.GetAsync(new Uri(string.Concat(this.Request.RequestUri.AbsoluteUri, "/", viewDefinition.Layout, "?data=", path, ".html")));

                                return result;
                            }
                        }

                    }

		        }

		        return new HttpResponseMessage(HttpStatusCode.NotFound);
		    }

		    object model;
            if (!string.IsNullOrEmpty(data))
                model = _modelProvider.GetModelForTemplate(templateInfo, data);
            else
                model = _modelProvider.GetDefaultModelForTemplate(templateInfo);

			return View(view, model);
		}
	}

    public class ViewDefinition
    {
        [JsonProperty("_layout")]
        public string Layout { get; set; }

        [JsonProperty("_placeholder")]
        public PlaceholderDefinitionCollection Placeholder { get; set; }
    }

    public class PlaceholderDefinitionCollection : Dictionary<string, PlaceholderDefinition[]>
    {
        
    }

    public class PlaceholderDefinition
    {
        [JsonProperty("template")]
        public string Template { get; set; }

        [JsonProperty("skin")]
        public string Skin { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }
    }
}
