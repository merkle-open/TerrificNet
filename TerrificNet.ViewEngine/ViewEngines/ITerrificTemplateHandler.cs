namespace TerrificNet.ViewEngine.ViewEngines
{
    public interface ITerrificTemplateHandler
    {
        string RenderPlaceholder(object model, string key, RenderingContext context);
        string RenderModule(string templateName, string skin, RenderingContext context);
    }
}