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
        private readonly IModelTypeProvider _modelTypeProvider;
	    private readonly IModuleRepository _moduleRepository;

	    public TerrificNetViewEngine(IViewEngineTerrific viewEngine, ITemplateRepository templateRepository, IModelTypeProvider modelTypeProvider, IModuleRepository moduleRepository)
        {
            _viewEngine = viewEngine;
            _templateRepository = templateRepository;
            _modelTypeProvider = modelTypeProvider;
	        _moduleRepository = moduleRepository;
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
			TemplateInfo templateInfo = null;
	        ModuleDefinition moduleDefinition;
			if ((_moduleRepository.TryGetModuleDefinitionById(viewName, out moduleDefinition) ||
				 _moduleRepository.TryGetModuleDefinitionById(controllerName, out moduleDefinition)))
			{
				templateInfo = moduleDefinition.DefaultTemplate;
			}

            if (templateInfo != null || (_templateRepository.TryGetTemplate(viewName, out templateInfo) || _templateRepository.TryGetTemplate(controllerName, out templateInfo)))
            {
                Type modelType;
                if (!_modelTypeProvider.TryGetModelTypeFromTemplate(templateInfo, out modelType))
                    modelType = typeof (object);

                IViewTerrific view;
                if (_viewEngine.TryCreateView(templateInfo, modelType, out view))
					return new ViewEngineResult(new TerrificViewAdapter(CreateAdapter(view), ResolveContext), this);
            }

            return new ViewEngineResult(new[] { viewName, controllerName });
        }

	    protected virtual IViewTerrific CreateAdapter(IViewTerrific view)
	    {
		    return view;
	    }

	    protected virtual MvcRenderingContext ResolveContext(ViewContext viewContext, IViewDataContainer viewDataContainer)
	    {
		    return MvcRenderingContext.Build(viewContext, viewDataContainer);
	    }

        private class TerrificViewAdapter : IView, IViewDataContainer
        {
            private readonly IViewTerrific _adaptee;
			private readonly Func<ViewContext, IViewDataContainer, RenderingContext> _resolveContext;

	        public TerrificViewAdapter(IViewTerrific adaptee, Func<ViewContext, IViewDataContainer, RenderingContext> resolveContext)
            {
	            _adaptee = adaptee;
	            _resolveContext = resolveContext;
            }

	        public void Render(ViewContext viewContext, TextWriter writer)
            {
                this.ViewData = viewContext.ViewData;
		        var context = _resolveContext(viewContext, this);
	            _adaptee.Render(viewContext.ViewData.Model, context);
            }

	        public ViewDataDictionary ViewData { get; set; }
        }
    }
}
