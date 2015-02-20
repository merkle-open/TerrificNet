using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.Config;
using TerrificNet.ViewEngine.ViewEngines.TemplateHandler;

namespace TerrificNet.Controllers
{
    public class TemplateController : TemplateControllerBase
    {
        private readonly IViewEngine _viewEngine;
        private readonly IModelProvider _modelProvider;
        private readonly ITemplateRepository _templateRepository;
        private readonly TerrificViewDefinitionRepository _viewRepository;

        public TemplateController(IViewEngine viewEngine, IModelProvider modelProvider, ITemplateRepository templateRepository, TerrificViewDefinitionRepository viewRepository)
        {
            _viewEngine = viewEngine;
            _modelProvider = modelProvider;
            _templateRepository = templateRepository;
            _viewRepository = viewRepository;
        }

        [HttpGet]
        public HttpResponseMessage Get(string path, string data = null)
        {
            IView view;
            TemplateInfo templateInfo;
            if (!_templateRepository.TryGetTemplate(path, out templateInfo) ||
                !_viewEngine.TryCreateView(templateInfo, out view))
            {
                PageViewDefinition viewDefinition;
                if (_viewRepository.TryGetFromView(path, out viewDefinition))
                    return Get(viewDefinition.Template, JObject.FromObject(viewDefinition));

                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            object model;
            if (!string.IsNullOrEmpty(data))
                model = _modelProvider.GetModelForTemplate(templateInfo, data);
            else
                model = _modelProvider.GetDefaultModelForTemplate(templateInfo);

            return View(view, model);
        }

        private HttpResponseMessage Get(string path, object data)
        {
            IView view;
            TemplateInfo templateInfo;
            if (!_templateRepository.TryGetTemplate(path, out templateInfo) ||
                !_viewEngine.TryCreateView(templateInfo, out view))
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            return View(view, data);
        }
    }

    public class TerrificViewDefinitionRepository
    {
        private readonly IFileSystem _fileSystem;
        private readonly ITerrificNetConfig _configuration;

        public TerrificViewDefinitionRepository(IFileSystem fileSystem, ITerrificNetConfig configuration)
        {
            _fileSystem = fileSystem;
            _configuration = configuration;
        }

        public bool TryGetFromView(string path, out PageViewDefinition viewDefinition)
        {
            var fileName = _fileSystem.Path.ChangeExtension(_fileSystem.Path.Combine(_configuration.ViewPath, path), "html.json");
            if (_fileSystem.FileExists(fileName))
            {
                if (TryReadPageDefinition(out viewDefinition, fileName)) 
                    return true;
            }

            viewDefinition = null;
            return false;
        }

        public IEnumerable<PageViewDefinition> GetAll()
        {
            foreach (var viewPath in _fileSystem.DirectoryGetFiles(_configuration.ViewPath, "html.json"))
            {
                PageViewDefinition viewDefinition;
                if (TryReadPageDefinition(out viewDefinition, viewPath))
                    yield return viewDefinition;
            }
        }

        private bool TryReadPageDefinition(out PageViewDefinition viewDefinition, string fileName)
        {
            using (var reader = new JsonTextReader(new StreamReader(_fileSystem.OpenRead(fileName))))
            {
                var jObj = JToken.ReadFrom(reader);
                viewDefinition = ViewDefinition.FromJObject<PageViewDefinition>(jObj);
                if (viewDefinition != null)
                {
                    viewDefinition.Id = fileName;
                    return true;
                }
            }
            return false;
        }
    }
}
