using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using System.Web.Http.Dispatcher;
using Microsoft.Practices.Unity;
using TerrificNet.UnityModule;
using Unity.WebApi;

namespace TerrificNet
{
    public class ApplicationSpecificControllerSelector : DefaultHttpControllerSelector
    {
        private readonly IUnityContainer _container;
        private readonly Lazy<Dictionary<string, IDependencyResolver>> _containers;

        public ApplicationSpecificControllerSelector(IUnityContainer container, HttpConfiguration configuration) : base(configuration)
        {
            _container = container;
            _containers = new Lazy<Dictionary<string, IDependencyResolver>>(GetContainers);
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            var applications = _containers.Value;
            var section = (string)request.GetRouteData().Route.Defaults["section"] ?? string.Empty;

            IDependencyResolver container;
            if (!applications.TryGetValue(section, out container))
                throw new InvalidOperationException(string.Format("Could not find a application for the section '{0}'.", section));

            var description = base.SelectController(request);
            description.Configuration.DependencyResolver = container;
            
            return description;
        }

        private Dictionary<string, IDependencyResolver> GetContainers()
        {
            return _container.ResolveAll<TerrificNetApplication>().ToDictionary(r => r.Section, r => (IDependencyResolver) new UnityDependencyResolver(r.Container));
        }
    }
}