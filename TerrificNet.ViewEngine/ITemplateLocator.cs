namespace TerrificNet.ViewEngine
{
	public interface ITemplateLocator
	{
		bool TryLocateTemplate(string name, out string path);
	}
}