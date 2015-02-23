using System;

namespace TerrificNet.Generator.MSBuild
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var source = @"D:\projects\TerrificNet\TerrificNet.Sample";
            Console.Write(GeneratorUtility.ExecuteString(source));
            //CompileTask.Execute(source, @"C:\Users\mschaelle\Source\Repos\TerrificNet\TerrificNet.Sample.Net.Models\bin\Debug\Models.dll");
            //CompileTask.ExecuteFile(source, @"C:\Users\mschaelle\Source\Repos\TerrificNet\TerrificNet.Sample.Net.Models\bin\Debug\Models.cs");
            Console.ReadLine();
        }
    }
}
