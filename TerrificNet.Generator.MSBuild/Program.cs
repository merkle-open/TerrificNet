using System;

namespace TerrificNet.Generator.MSBuild
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var source = @"D:\projects\TerrificNet\TerrificNet.Sample";
            Console.Write(GeneratorUtility.ExecuteStringAsync(source));
            //CompileTask.ExecuteAsync(source, @"C:\Users\mschaelle\Source\Repos\TerrificNet\TerrificNet.Sample.Net.Models\bin\Debug\Models.dll");
            //CompileTask.ExecuteFileAsync(source, @"C:\Users\mschaelle\Source\Repos\TerrificNet\TerrificNet.Sample.Net.Models\bin\Debug\Models.cs");
            Console.ReadLine();
        }
    }
}
