namespace TerrificNet.Controller
{
    public interface IModelProvider
    {
        object GetModelFromPath(string path);
    }
}