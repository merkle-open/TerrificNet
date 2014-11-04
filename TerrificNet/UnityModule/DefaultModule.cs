using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using TerrificNet.Config;
using TerrificNet.Controller;

namespace TerrificNet.UnityModule
{
	public class DefaultUnityModule : IUnityModue
	{
		private readonly string _path;

		public DefaultUnityModule(string path)
		{
			_path = path;
		}

		public void Configure(IUnityContainer container)
		{
			container.RegisterInstance<ITerrificNetConfig>(new TerrificNetConfig { Path = _path });
            container.RegisterType<IViewEngine, PhysicalViewEngine>(new InjectionConstructor(Path.Combine(_path, "views")));
            container.RegisterType<IModelProvider, JsonModelProvier>(new InjectionConstructor(Path.Combine(_path, "project/data")));
		}

		private class TerrificNetConfig : ITerrificNetConfig
		{
			public string Path { get; set; }
		}
	}

	public interface IUnityModue
	{
		void Configure(IUnityContainer container);
	}
}
