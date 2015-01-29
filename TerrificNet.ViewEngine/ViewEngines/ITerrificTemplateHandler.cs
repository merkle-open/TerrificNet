namespace TerrificNet.ViewEngine.ViewEngines
{
    public interface ITerrificTemplateHandler
    {
        void RenderPlaceholder(object model, string key, RenderingContext context);
        void RenderModule(string templateName, string skin, RenderingContext context);
		void RenderLabel(string key, RenderingContext context);
    }
}