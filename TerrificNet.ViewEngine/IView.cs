using Veil;

namespace TerrificNet.ViewEngine
{
    public interface IView : IView<object>
    {
    }

    public interface IView<in T>
    {
        void Render(T model, RenderingContext context);
    }
}