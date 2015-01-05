using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Microsoft.Practices.Unity;
using TerrificNet.UnityModule;

namespace TerrificNet
{
    public class ApplicationSpecificControllerActivator : IHttpControllerActivator
    {
        private readonly IUnityContainer _container;

        public ApplicationSpecificControllerActivator(IUnityContainer container)
        {
            _container = container;
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var applications = _container.ResolveAll<TerrificNetApplication>();
            var section = (string) request.GetRouteData().Route.Defaults["section"] ?? string.Empty;

            var application = applications.FirstOrDefault(a => a.Configuration.Section == section);
            if (application == null)
                throw new InvalidOperationException(string.Format("Could not find a application for the section '{0}'.", section));

            return (IHttpController) application.Container.Resolve(controllerType);
        }
    }
}