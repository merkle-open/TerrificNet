using System;
using System.IO;
using System.Linq;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Microsoft.Practices.Unity;
using Owin;

namespace TerrificNet.ConsoleHost
{
    class Program
    {
        private const string PathArgumentPrefix = "--path=";

        static void Main(string[] args)
        {
            const string baseAddress = "http://+:9000/";

            var path = args.FirstOrDefault(i => i.StartsWith(PathArgumentPrefix));
            if (!string.IsNullOrEmpty(path))
                path = path.Substring(PathArgumentPrefix.Length);
            else
                path = ".";

            var container = WebInitializer.Initialize(Path.GetFullPath(path));

            // Start OWIN host
            using (WebApp.Start(baseAddress, builder => Initialize(builder, container)))
            {
                Console.WriteLine("Started on " + baseAddress);
                Console.ReadLine();
            }
        }

        private static void Initialize(IAppBuilder builder, IUnityContainer container)
        {
            var config = new HttpConfiguration();
	        
			new Startup().Configuration(container, config);
            builder.UseWebApi(config);
        }
    }
}
