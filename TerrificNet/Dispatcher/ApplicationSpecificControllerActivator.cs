using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using System.Web.Http.Dispatcher;
using TerrificNet.Controllers;
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
            string section = GetSectionName(request);
            IDependencyResolver container;
            if (!this._containers.Value.TryGetValue(section, out container))
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

        private static string GetSectionName(HttpRequestMessage request)
        {
            return ((string)request.GetRouteData().Route.Defaults["section"]) ?? string.Empty;
        }

        private Dictionary<string, IDependencyResolver> GetContainers()
        {
            return this._configuration.DependencyResolver.GetServices(typeof(TerrificNetApplication))
                .OfType<TerrificNetApplication>()
                .ToDictionary(r => r.Section, r => (IDependencyResolver)new UnityDependencyResolver(r.Container));
        }
    }
}
