namespace TerrificNet.ViewEngine.IO
{
	public interface IPathHelper
	{
		string Combine(params string[] parts);
		string GetDirectoryName(string filePath);
		string ChangeExtension(string fileName, string extension);
		string GetFileNameWithoutExtension(string path);
		string GetExtension(string path);
	}
}