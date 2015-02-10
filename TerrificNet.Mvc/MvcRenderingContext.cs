using System.Web.Mvc;
using TerrificNet.ViewEngine;

namespace TerrificNet.Mvc
{
    public class MvcRenderingContext : RenderingContext
    {
	    private const string ContextKey = "_mvcrenderingcontext";
	    public ViewContext ViewContext { get; private set; }
        public IViewDataContainer ViewDataContainer { get; private set; }

	    private MvcRenderingContext(ViewContext viewContext, IViewDataContainer viewDataContainer, RenderingContext parentContext) : base(viewContext.Writer)
        {
            ViewContext = viewContext;
            ViewDataContainer = viewDataContainer;

	        if (parentContext != null)
	        {
		        foreach (var dataEntry in parentContext.Data)
			        this.Data.Add(dataEntry);
	        }
        }

		internal static MvcRenderingContext Build(ViewContext viewContext, IViewDataContainer viewDataContainer)
		{
			var context = GetFromViewContext(viewContext);
			if (context != null)
				return context;

			MvcRenderingContext parentContext = null; 
			if (viewContext.ParentActionViewContext != null)
				parentContext = GetFromViewContext(viewContext.ParentActionViewContext);

			context = new MvcRenderingContext(viewContext, viewDataContainer, parentContext);
			viewContext.ViewData.Add(ContextKey, context);

			return context;
		}

	    public static MvcRenderingContext GetFromViewContext(ViewContext viewContext)
	    {
		    object contextObj;
		    MvcRenderingContext context = null;
		    if (viewContext.ViewData.TryGetValue(ContextKey, out contextObj))
			    context = contextObj as MvcRenderingContext;

		    return context;
	    }
    }
}