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

        public AssetCompilerFactory(IAssetCompiler[] compiler, IUnityContainer container)
        {
            _container = container;
            foreach (var c in compiler)
            {
                var localCompiler = c;
                _compiler.Add(localCompiler.CanProcess, () => _container.Resolve(localCompiler.GetType()) as IAssetCompiler);
            }
        }

        public IAssetCompiler GetCompiler(string assetName)
        {
            return _compiler.First(o => o.Key(assetName)).Value();
        }
    }
}
