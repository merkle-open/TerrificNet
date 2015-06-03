using System.IO;
using System.Threading.Tasks;

namespace TerrificNet.ViewEngine.TemplateHandler.UI
{
    public interface IPageViewDefinition : IPartialViewDefinition
    {
        string Id { get; set; }

        void Render(IViewEngine engine, StreamWriter writer);
    }
}