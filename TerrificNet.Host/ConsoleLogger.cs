using System;
using NuGet;

namespace TerrificNet.Host
{
    public class ConsoleLogger : ILogger
    {
        public FileConflictResolution ResolveFileConflict(string message)
        {
            return FileConflictResolution.OverwriteAll;
        }

        public void Log(MessageLevel level, string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }
    }
}