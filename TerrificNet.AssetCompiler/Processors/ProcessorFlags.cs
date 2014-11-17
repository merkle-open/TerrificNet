using System;

namespace TerrificNet.AssetCompiler.Processors
{
    [Flags]
    public enum ProcessorFlags
    {
        Minify = 1,
        SourceMap = 2,
        None = 0
    }
}
