namespace TerrificNet.ViewEngine
{
    public interface IViewEngine
    {
        bool TryCreateView(TemplateInfo templateInfo, out IView view);
        IView CreateView(string content);
    }
}