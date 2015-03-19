using System.Threading.Tasks;
using Veil;

namespace TerrificNet.ViewEngine.TemplateHandler
{
    public interface ITerrificTemplateHandler
    {
        Task RenderPlaceholderAsync(object model, string key, RenderingContext context);
        Task RenderModuleAsync(string moduleId, string skin, RenderingContext context);
        Task RenderLabelAsync(string key, RenderingContext context);
        Task RenderPartialAsync(string template, object model, RenderingContext context);
    }
}