namespace TerrificNet.ViewEngine.TemplateHandler.UI
{
    public interface IPartialViewDefinition
    {
        string Template { get; }
        object Data { get; }
    }
}