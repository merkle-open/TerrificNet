namespace TerrificNet.ViewEngine
{
    public interface IModelProvider
    {
        object GetModelFromPath(string path);
        void UpdateModelFromPath(string path, object content);
    }
}