namespace TerrificNet.ViewEngine
{
    public interface IModelProvider
    {
        object GetDefaultModelForTemplate(TemplateInfo template);
        void UpdateDefaultModelForTemplate(TemplateInfo template, object content);

        object GetModelForTemplate(TemplateInfo template, string dataId);
    }
}