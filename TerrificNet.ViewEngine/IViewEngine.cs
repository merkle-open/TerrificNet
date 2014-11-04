namespace TerrificNet.ViewEngine
{
    public interface IViewEngine
    {
        bool TryCreateViewFromPath(string path, out IView view);
    }
}