using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;

namespace TerrificNet
{
	class Program
	{
		static void Main(string[] args)
		{
			const string baseAddress = "http://+:9000/";

			// Start OWIN host 
			using (WebApp.Start<Startup>(url: baseAddress))
			{
				Console.WriteLine("Started on " + baseAddress);
				Console.ReadLine();
			}
		}
	}
}
