using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TerrificNet.AssetCompiler.Compiler
{
    public class AssetCompilerFactory : IAssetCompilerFactory
    {
        private readonly IUnityContainer _container;
        private readonly Dictionary<Func<string, bool>, Func<IAssetCompiler>> _compiler = new Dictionary<Func<string, bool>, Func<IAssetCompiler>>();

        public AssetCompilerFactory(IEnumerable<IAssetCompiler> compiler, IUnityContainer container)
        {
            _container = container;
            foreach (var c in compiler)
            {
                _compiler.Add(i => c.CanProcess(i), () =>c);
            }
        }

        public IAssetCompiler GetCompiler(string assetName)
        {
            return _compiler.First(o => o.Key(assetName)).Value();
        }
    }
}
