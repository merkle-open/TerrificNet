using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TerrificNet.Models;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.Config;

namespace TerrificNet.Controller
{
    public class ViewIndexController : TemplateControllerBase
    {
        private readonly IViewEngine _viewEngine;
        private readonly ITerrificNetConfig _configuration;

        public ViewIndexController(IViewEngine viewEngine, ITerrificNetConfig configuration)
        {
            _viewEngine = viewEngine;
            _configuration = configuration;
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            var model = new ViewOverviewModel
            {
                Views = GetViews().ToList()
            };

            IView view;
            if (!_viewEngine.TryCreateViewFromPath("index.html", out view))
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            return Render(view, model);
        }

        private IEnumerable<ViewItemModel> GetViews()
        {
            foreach (var file in Directory.GetFiles(_configuration.ViewPath))
            {
                yield return new ViewItemModel
                {
                    Text = Path.GetFileNameWithoutExtension(file),
                    Url = string.Format("/{0}", Path.GetFileName(file)),
                    EditUrl = string.Format("/web/edit.html?template={0}", Path.GetFileName(file))
                };
            }
        }
    }
}