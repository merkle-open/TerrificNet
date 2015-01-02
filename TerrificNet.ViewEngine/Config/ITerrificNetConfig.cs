namespace TerrificNet.ViewEngine.Config
{
	public interface ITerrificNetConfig
	{
		string BasePath { get; }
		string ViewPath { get; }
		string ModulePath { get; }
		string AssetPath { get; }
		string DataPath { get; }
	}
}
