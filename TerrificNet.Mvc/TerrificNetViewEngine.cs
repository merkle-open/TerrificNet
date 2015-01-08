using System;
using System.IO;
using System.Web.Mvc;
using TerrificNet.ViewEngine;
using IView = System.Web.Mvc.IView;
using IViewEngine = System.Web.Mvc.IViewEngine;
using IViewEngineTerrific = TerrificNet.ViewEngine.IViewEngine;
using IViewTerrific = TerrificNet.ViewEngine.IView;
using TemplateInfo = TerrificNet.ViewEngine.TemplateInfo;

namespace TerrificNet.Mvc
{
    public class TerrificNetViewEngine : IViewEngine
    {
        private readonly IViewEngineTerrific _viewEngine;
        private readonly ITemplateRepository _templateRepository;

        public TerrificNetViewEngine(IViewEngineTerrific viewEngine, ITemplateRepository templateRepository)
        {
            _viewEngine = viewEngine;
            _templateRepository = templateRepository;
        }

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            return GetViewResult(controllerContext.RouteData.Values["controller"].ToString(), partialViewName);
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            return GetViewResult(controllerContext.RouteData.Values["controller"].ToString(), viewName);
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
        }

        private ViewEngineResult GetViewResult(string controllerName, string viewName)
        {
            TemplateInfo templateInfo;
            IViewTerrific view;
            if ((_templateRepository.TryGetTemplate(viewName, string.Empty, out templateInfo) || _templateRepository.TryGetTemplate(controllerName, string.Empty, out templateInfo))
                && _viewEngine.TryCreateView(templateInfo, out view))
                return new ViewEngineResult(new TerrificViewAdapter(view), this);

            throw new NotSupportedException();
        }

        private class TerrificViewAdapter : IView, IViewDataContainer
        {
            private readonly IViewTerrific _adaptee;

            public TerrificViewAdapter(IViewTerrific adaptee)
            {
                _adaptee = adaptee;
            }

            public void Render(ViewContext viewContext, TextWriter writer)
            {
                this.ViewData = viewContext.ViewData;
                writer.Write(_adaptee.Render(viewContext.ViewData.Model, new MvcRenderingContext(viewContext, this)));
            }

            public ViewDataDictionary ViewData { get; set; }
        }
    }
}
