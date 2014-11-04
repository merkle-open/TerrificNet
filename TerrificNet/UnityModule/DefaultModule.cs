using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using TerrificNet.Controller;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.Config;
using TerrificNet.ViewEngine.ModelProviders;
using TerrificNet.ViewEngine.SchemaProviders;
using TerrificNet.ViewEngine.ViewEngines;

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
			container.RegisterInstance<ITerrificNetConfig>(new TerrificNetConfig
			{
				BasePath = _path,
				ViewPath = Path.Combine(_path, "views"),
				AssetPath = Path.Combine(_path, "assets"),
				DataPath = Path.Combine(_path, "project/data"),
			});

			container.RegisterType<IViewEngine, NustachePhysicalViewEngine>();
			container.RegisterType<IModelProvider,JsonModelProvier>();
			container.RegisterType<ISchemaProvider, NustacheViewSchemaProvider>();
		}

		private class TerrificNetConfig : ITerrificNetConfig
		{
			public string BasePath { get; set; }
			public string ViewPath { get; set; }
			public string AssetPath { get; set; }
			public string DataPath { get; set; }
		}
	}

	public interface IUnityModue
	{
		void Configure(IUnityContainer container);
	}
}
