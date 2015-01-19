namespace TerrificNet.ViewEngine
{
    public interface IView
    {
        void Render(object model, RenderingContext context);
    }
}