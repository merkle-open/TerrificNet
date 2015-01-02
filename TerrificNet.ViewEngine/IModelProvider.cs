namespace TerrificNet.ViewEngine
{
    public interface IModelProvider
    {
        object GetModelForTemplate(string template);
        void UpdateModelForTemplate(string template, object content);
    }
}