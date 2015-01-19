using System.Web.Mvc;
using TerrificNet.ViewEngine;

namespace TerrificNet.Mvc
{
    public class MvcRenderingContext : RenderingContext
    {
        public ViewContext ViewContext { get; private set; }
        public IViewDataContainer ViewDataContainer { get; private set; }

        public MvcRenderingContext(ViewContext viewContext, IViewDataContainer viewDataContainer) : base(viewContext.Writer)
        {
            ViewContext = viewContext;
            ViewDataContainer = viewDataContainer;
        }
    }
}