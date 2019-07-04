using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Src
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               "Page",
               "Page/{link}",
               new { controller = "Page", action = "Index" },
               new[] { "Src.Controllers" }
           );

            routes.MapRoute(
                "Search",
                "Search",
                new { controller = "Product", action = "Search" },
                new[] { "Src.Controllers" }
            );

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "Src.Controllers" }
            );
        }
    }
}
