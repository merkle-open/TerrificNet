using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrificNet.AssetCompiler.Configuration;

namespace TerrificNet.AssetCompiler.Bundler
{
    public class LocalAssetBundler : IAssetBundler
    {
        public async Task<string> BundleAsync(AssetComponents components)
        {
            var results =
                await
                    Task.WhenAll(
                        Task.WhenAll(components.Dependencies.Select(o => o())),
                        Task.WhenAll(components.Files.Select(o => o()))
                        );
            var sb = new StringBuilder();
            foreach (var res in results)
            {
                res.Aggregate(sb, (builder, s) => builder.AppendLine(s));
            }
            return sb.ToString();
        }
    }
}
