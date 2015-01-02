using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Practices.Unity;
using TerrificNet.Models;
using TerrificNet.UnityModule;
using TerrificNet.ViewEngine;

namespace TerrificNet.Controller
{
    public class ViewIndexController : TemplateControllerBase
    {
        private readonly IViewEngine _viewEngine;
        private readonly ITemplateRepository _templateRepository;
        private readonly TerrificNetApplication[] _applications;

        public ViewIndexController(
            IViewEngine viewEngine, 
            ITemplateRepository templateRepository,
            TerrificNetApplication[] applications)
        {
            _viewEngine = viewEngine;
            _templateRepository = templateRepository;
            _applications = applications;
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            var model = new ViewOverviewModel
            {
                Views = _applications.SelectMany(a => GetViews(a.Container.Resolve<ITemplateRepository>())).ToList()
            };

            IView view;
            TemplateInfo templateInfo;
            if (!_templateRepository.TryGetTemplate("index", out templateInfo) ||
                !_viewEngine.TryCreateView(templateInfo, out view))
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            return Render(view, model);
        }

        private IEnumerable<ViewItemModel> GetViews(ITemplateRepository templateRepository)
        {
            foreach (var file in templateRepository.GetAll())
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