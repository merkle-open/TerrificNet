using System.Threading.Tasks;

namespace TerrificNet.AssetCompiler
{
    public interface IAssetCompiler
    {
        Task<string> CompileAsync(string content, bool minify);

        bool CanProcess(string filename);

	    string MimeType { get; }
    }
}
