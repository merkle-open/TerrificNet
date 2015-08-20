using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using TerrificNet.Mvc;

namespace TerrificNet.Sample.Net
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(DependencyResolver.Current.GetService<TerrificNetViewEngine>());

            var factory = ControllerBuilder.Current.GetControllerFactory();
            ControllerBuilder.Current.SetControllerFactory(new FallbackControllerFactory(factory));
        }

        private class FallbackControllerFactory : IControllerFactory
        {
            private readonly IControllerFactory _innerFactory;

            public FallbackControllerFactory(IControllerFactory innerFactory)
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
