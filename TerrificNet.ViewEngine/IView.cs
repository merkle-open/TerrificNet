namespace TerrificNet.ViewEngine
{
    public interface IView
    {
        string Render(object model, RenderingContext context);
    }
}