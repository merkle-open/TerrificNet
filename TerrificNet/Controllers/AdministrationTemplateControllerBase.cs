using System.Linq;
using Microsoft.Practices.Unity;
using TerrificNet.UnityModules;

namespace TerrificNet.Controllers
{
    public class AdministrationTemplateControllerBase : TemplateControllerBase
    {
        private readonly TerrificNetApplication[] _applications;

        protected AdministrationTemplateControllerBase(TerrificNetApplication[] applications)
        {
            _applications = applications;
        }

        protected T ResolveForApp<T>(string applicationName)
        {
            applicationName = applicationName ?? string.Empty;
            var application = _applications.First(a => a.Section == applicationName);

            return application.Container.Resolve<T>();
        }
    }
}