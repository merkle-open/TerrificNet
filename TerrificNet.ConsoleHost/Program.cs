using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Microsoft.Practices.Unity;
using Owin;
using TerrificNet.ViewEngine;

namespace TerrificNet.ConsoleHost
{
    class Program
    {
        private const string PathArgumentPrefix = "--path=";
        private const string BindingArgumentPrefix = "--bind=";

        static void Main(string[] args)
        {
            const string baseAddress = "http://+:9000/";

			var bind = args.FirstOrDefault(i => i.StartsWith(BindingArgumentPrefix));
			bind = !string.IsNullOrEmpty(bind) ? bind.Substring(PathArgumentPrefix.Length) : baseAddress;

            var path = args.FirstOrDefault(i => i.StartsWith(PathArgumentPrefix));
            path = !string.IsNullOrEmpty(path) ? path.Substring(PathArgumentPrefix.Length) : ".";

            var container = WebInitializer.Initialize(Path.GetFullPath(path));
	        container.RegisterType<IModelTypeProvider, DummyModelTypeProvider>();
			
			
            // Start OWIN host
			using (WebApp.Start(bind, builder => Initialize(builder, container)))
            {
				Console.WriteLine("Started on " + bind);
                Console.ReadLine();
            }
        }

        private static void Initialize(IAppBuilder builder, IUnityContainer container)
        {
            var config = new HttpConfiguration();
	        
			new Startup().Configuration(container, config);
            builder.UseWebApi(config);
        }

		private class DummyModelTypeProvider : IModelTypeProvider
		{
			public Task<Type> GetModelTypeFromTemplateAsync(TemplateInfo templateInfo)
			{
				return Task.FromResult<Type>(typeof(object));
			}
		}
    }
}
