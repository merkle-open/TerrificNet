using Microsoft.Practices.Unity;
using TerrificNet.ViewEngine.Config;

namespace TerrificNet.UnityModule
{
    public class TerrificNetApplication
    {
        public string Name { get; private set; }
        public string Section { get; private set; }
        public ITerrificNetConfig Configuration { get; private set; }
        public IUnityContainer Container { get; private set; }

        public TerrificNetApplication(string name, string section, ITerrificNetConfig configuration, IUnityContainer container)
        {
            Name = name;
            Section = section;
            Configuration = configuration;
            Container = container;
        }
    }
}