using Src.Controllers.Api;
using System.Web.Http;

namespace Src
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.DependencyResolver = new NinjectApi();

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/v1/{controller}/{action}/{token}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
