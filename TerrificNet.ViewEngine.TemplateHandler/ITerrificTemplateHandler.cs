using System.Threading.Tasks;
using Veil;

namespace TerrificNet.ViewEngine.TemplateHandler
{
    public interface ITerrificTemplateHandler
    {
        void RenderPlaceholder(object model, string key, string index, RenderingContext context);
        
		void RenderModule(string moduleId, string skin, string dataVariation, object model, RenderingContext context);
        
		void RenderLabel(string key, RenderingContext context);
        
		void RenderPartial(string template, object model, RenderingContext context);
    }
}