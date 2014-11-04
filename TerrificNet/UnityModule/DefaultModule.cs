using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using TerrificNet.Config;

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
