using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TerrificNet.Models;
using TerrificNet.ViewEngine;

namespace TerrificNet.Controller
{
    public class ViewIndexController : TemplateControllerBase
    {
        private readonly IViewEngine _viewEngine;
        private readonly ITemplateRepository _templateRepository;

        public ViewIndexController(
            IViewEngine viewEngine, 
            ITemplateRepository templateRepository)
        {
            _viewEngine = viewEngine;
            _templateRepository = templateRepository;
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            var model = new ViewOverviewModel
            {
                Views = GetViews().ToList()
            };

            IView view;
            TemplateInfo templateInfo;
            if (!_templateRepository.TryGetTemplate("index", out templateInfo) ||
                !_viewEngine.TryCreateView(templateInfo, out view))
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            return Render(view, model);
        }

        private IEnumerable<ViewItemModel> GetViews()
        {
            foreach (var file in _templateRepository.GetAll())
            {
                yield return new ViewItemModel
                {
                    Text = file.Id,
                    Url = string.Format("/{0}", file.Id),
                    EditUrl = string.Format("/web/edit.html?template={0}", file.Id),
                    SchemaUrl = string.Format("/schema/{0}", file.Id)
                };
            }
        }
    }
}