namespace TerrificNet.ViewEngine.Globalization
{
    public interface ILabelService
    {
	    string Get(string key);
	    void Remove(string key);
	    void Set(string key, string value);
    }
}
