namespace TerrificNet.ViewEngine
{
    public interface IModelProvider
    {
        object GetModelForTemplate(TemplateInfo template);
        void UpdateModelForTemplate(TemplateInfo template, object content);
    }
}