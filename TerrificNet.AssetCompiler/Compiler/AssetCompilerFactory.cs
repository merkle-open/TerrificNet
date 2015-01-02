using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TerrificNet.AssetCompiler.Compiler
{
    public class AssetCompilerFactory : IAssetCompilerFactory
    {
        private readonly Dictionary<Func<string, bool>, Func<IAssetCompiler>> _compiler = 
			new Dictionary<Func<string, bool>, Func<IAssetCompiler>>();

        public AssetCompilerFactory(IAssetCompiler[] compilers, IUnityContainer container)
        {
            foreach (var c in compilers)
            {
                var localCompiler = c;
                _compiler.Add(localCompiler.CanProcess, () => container.Resolve(localCompiler.GetType()) as IAssetCompiler);
            }
        }

        public IAssetCompiler GetCompiler(string assetName)
        {
            return _compiler.First(o => o.Key(assetName)).Value();
        }
    }
}
