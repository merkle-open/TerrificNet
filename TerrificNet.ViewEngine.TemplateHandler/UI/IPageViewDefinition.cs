using System.IO;
using System.Threading.Tasks;

namespace TerrificNet.ViewEngine.TemplateHandler.UI
{
    public interface IPageViewDefinition : IPartialViewDefinition
    {
        string Id { get; set; }

        Task RenderAsync(IViewEngine engine, StreamWriter writer);
    }
}