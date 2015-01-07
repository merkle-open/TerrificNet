using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrificNet.Generator.MSBuild
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CompileTask.Execute(@"C:\Users\mschaelle\Source\Repos\TerrificNet\TerrificNet.Sample", @"C:\Users\mschaelle\Source\Repos\TerrificNet\TerrificNet.Sample.Net.Models\bin\Debug\Models.dll");
            CompileTask.ExecuteFile(@"C:\Users\mschaelle\Source\Repos\TerrificNet\TerrificNet.Sample", @"C:\Users\mschaelle\Source\Repos\TerrificNet\TerrificNet.Sample.Net.Models\bin\Debug\Models.cs");
        }
    }
}
