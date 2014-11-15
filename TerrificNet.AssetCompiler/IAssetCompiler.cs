﻿using System.Threading.Tasks;

namespace TerrificNet.AssetCompiler
{
    public interface IAssetCompiler
    {
        Task<string> CompileAsync(string content);
    }
}