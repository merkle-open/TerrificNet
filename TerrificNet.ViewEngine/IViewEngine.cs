namespace TerrificNet.Controller
{
    public interface IViewEngine
    {
        bool TryCreateViewFromPath(string path, out IView view);
    }
}