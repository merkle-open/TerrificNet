using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using Microsoft.Practices.Unity;
using TerrificNet.Mvc;
using TerrificNet.Sample.Net.Controllers;

namespace TerrificNet.Sample.Net
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var container = Bootstrapper.Initialise();

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(container.Resolve<TerrificNetViewEngine>());

            var factory = ControllerBuilder.Current.GetControllerFactory();
            ControllerBuilder.Current.SetControllerFactory(new Test(factory));
        }

        private class Test : IControllerFactory
        {
            private readonly IControllerFactory _innerFactory;

            public Test(IControllerFactory innerFactory)
            {
                _innerFactory = innerFactory;
            }

            public IController CreateController(RequestContext requestContext, string controllerName)
            {
                try
                {
                    return _innerFactory.CreateController(requestContext, controllerName);
                }
                catch (HttpException)
                {
                    return _innerFactory.CreateController(requestContext, "Fallback");
                }
            }

            public SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, string controllerName)
            {
                return _innerFactory.GetControllerSessionBehavior(requestContext, controllerName);
            }

            public void ReleaseController(IController controller)
            {
                _innerFactory.ReleaseController(controller);
            }
        }
    }
}
