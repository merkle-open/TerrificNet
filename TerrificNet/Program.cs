using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Microsoft.Practices.Unity;
using TerrificNet.UnityModule;

namespace TerrificNet
{
	class Program
	{
		private const string PathArgumentPrefix = "--path=";

		static void Main(string[] args)
		{
			const string baseAddress = "http://+:9000/";

			var path =args.FirstOrDefault(i => i.StartsWith(PathArgumentPrefix));
			if (!string.IsNullOrEmpty(path))
				path = path.Substring(PathArgumentPrefix.Length);

			var container = new UnityContainer();
			new DefaultUnityModule().Configure(container);

			// Start OWIN host
			using (WebApp.Start(baseAddress, builder => new Startup().Configuration(builder, container)))
			{
				Console.WriteLine("Started on " + baseAddress);
				Console.ReadLine();
			}
		}
	}
}
