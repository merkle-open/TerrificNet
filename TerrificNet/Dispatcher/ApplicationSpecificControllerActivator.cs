using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using System.Web.Http.Dispatcher;
using TerrificNet.UnityModules;
using Unity.WebApi;

namespace TerrificNet.Dispatcher
{
    public class ApplicationSpecificControllerActivator : IHttpControllerActivator
    {
        private readonly HttpConfiguration _configuration;
        private readonly Lazy<Dictionary<string, IDependencyResolver>> _containers;

        public ApplicationSpecificControllerActivator(HttpConfiguration configuration)
        {
            _configuration = configuration;
            _containers = new Lazy<Dictionary<string, IDependencyResolver>>(GetContainers);
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            string section;
            if (!TryGetSectionName(request, out section))
                return (IHttpController)_configuration.DependencyResolver.GetService(controllerType);

            IDependencyResolver container;
            if (!_containers.Value.TryGetValue(section, out container))
            {
                throw new InvalidOperationException(string.Format("Could not find a application for the section '{0}'.", section));
            }

            var controller = (IHttpController)container.GetService(controllerType);
            var dpAware = controller as IDependencyResolverAware;
            if (dpAware != null)
            {
                dpAware.DependencyResolver = container;
            }
            return controller;
        }

        private static bool TryGetSectionName(HttpRequestMessage request, out string sectionName)
        {
            sectionName = null;
            var routeData = request.GetRouteData();
            if (routeData == null)
                return false;

            var defaults = routeData.Route.Defaults;
            if (defaults == null)
                return false;

            object value;
            if (!defaults.TryGetValue("section", out value))
                return false;

            sectionName = value as string ?? string.Empty;
            return true;
        }

        private Dictionary<string, IDependencyResolver> GetContainers()
        {
            return _configuration.DependencyResolver.GetServices(typeof(TerrificNetApplication))
                .OfType<TerrificNetApplication>()
                .ToDictionary(r => r.Section, r => (IDependencyResolver)new UnityDependencyResolver(r.Container));
        }
    }
}
