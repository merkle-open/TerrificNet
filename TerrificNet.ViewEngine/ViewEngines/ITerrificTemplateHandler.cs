namespace TerrificNet.ViewEngine.ViewEngines
{
    public interface ITerrificTemplateHandler
    {
        void RenderPlaceholder(object model, string key, RenderingContext context);
        void RenderModule(string moduleId, string skin, RenderingContext context);
		void RenderLabel(string key, RenderingContext context);
        void RenderPartial(string template, object model, RenderingContext context);
    }
}