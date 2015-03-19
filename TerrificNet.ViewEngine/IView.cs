using System.Threading.Tasks;
using Veil;

namespace TerrificNet.ViewEngine
{
    public interface IView : IView<object>
    {
    }

    public interface IView<in T>
    {
        Task RenderAsync(T model, RenderingContext context);
    }
}