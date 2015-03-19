using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;
using TerrificNet.ViewEngine;
using Veil;
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
            // TODO: check if async possible
            return GetViewResult(controllerContext.RouteData.Values["controller"].ToString(), partialViewName).Result;
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            // TODO: check if async possible
            return GetViewResult(controllerContext.RouteData.Values["controller"].ToString(), viewName).Result;
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
        }

        private async Task<ViewEngineResult> GetViewResult(string controllerName, string viewName)
        {
			TemplateInfo templateInfo = null;
            var moduleDefinition = await _moduleRepository.GetModuleDefinitionByIdAsync(viewName).ConfigureAwait(false);
            if (moduleDefinition == null)
                moduleDefinition = await _moduleRepository.GetModuleDefinitionByIdAsync(controllerName).ConfigureAwait(false);

			if (moduleDefinition != null)
				templateInfo = moduleDefinition.DefaultTemplate;

            if (templateInfo == null)
                templateInfo = await _templateRepository.GetTemplateAsync(viewName).ConfigureAwait(false);

            if (templateInfo == null)
                templateInfo = await _templateRepository.GetTemplateAsync(controllerName).ConfigureAwait(false);

            if (templateInfo != null)
            {
                var modelType = await _modelTypeProvider.GetModelTypeFromTemplateAsync(templateInfo).ConfigureAwait(false);
                if (modelType == null)
                    modelType = typeof (object);

                var view = await _viewEngine.CreateViewAsync(templateInfo, modelType).ConfigureAwait(false);
                if (view != null)
					return new ViewEngineResult(new TerrificViewAdapter(CreateAdapter(view), ResolveContext), this);
            }

            return new ViewEngineResult(new[] { viewName, controllerName });
        }

	    protected virtual IViewTerrific CreateAdapter(IViewTerrific view)
	    {
		    return view;
	    }

	    protected virtual MvcRenderingContext ResolveContext(ViewContext viewContext, IViewDataContainer viewDataContainer, TextWriter writer)
	    {
		    return MvcRenderingContext.Build(viewContext, viewDataContainer, writer);
	    }

        private class TerrificViewAdapter : IView, IViewDataContainer
        {
            private readonly IViewTerrific _adaptee;
			private readonly Func<ViewContext, IViewDataContainer, TextWriter, RenderingContext> _resolveContext;

	        public TerrificViewAdapter(IViewTerrific adaptee, Func<ViewContext, IViewDataContainer, TextWriter, RenderingContext> resolveContext)
            {
	            _adaptee = adaptee;
	            _resolveContext = resolveContext;
            }

	        public void Render(ViewContext viewContext, TextWriter writer)
            {
                this.ViewData = viewContext.ViewData;
		        var context = _resolveContext(viewContext, this, writer);
	            _adaptee.RenderAsync(viewContext.ViewData.Model, context);
            }

	        public ViewDataDictionary ViewData { get; set; }
        }
    }
}
