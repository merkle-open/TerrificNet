using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace TerrificNet.Dispatcher
{
    public class ApplicationSpecificControllerActivator : IHttpControllerActivator
    {
        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            return (IHttpController)controllerDescriptor.Configuration.DependencyResolver.GetService(controllerType);
        }
    }
}