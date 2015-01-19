using System;

namespace TerrificNet.ViewEngine
{
    public interface IViewEngine
    {
        bool TryCreateView(TemplateInfo templateInfo, Type modelType, out IView view);
    }

    public static class ViewEngineExtension
    {
        public static bool TryCreateView(this IViewEngine viewEngine, TemplateInfo templateInfo, out IView view)
        {
            return viewEngine.TryCreateView(templateInfo, typeof (object), out view);
        }
    }
}