using System.IO;

namespace TerrificNet.ViewEngine.ViewEngines
{
	public abstract class PhysicalViewEngineBase : IViewEngine
	{
		public bool TryCreateViewFromPath(string path, out IView view)
		{
			view = null;

			if (!File.Exists(path))
				return false;

			using (var reader = new StreamReader(path))
			{
				var content = reader.ReadToEnd();

				view = CreateView(content);
				return true;
			}
		}

		protected abstract IView CreateView(string content);
	}
}