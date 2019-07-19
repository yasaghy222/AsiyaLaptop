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
              "Product",
              "Product/{id}/{title}",
              new { controller = "Product", action = "Index", title = UrlParameter.Optional },
              new[] { "Src.Controllers" }
            );

            routes.MapRoute(
              "AddVisitCount",
              "AddVisitCount",
              new { controller = "Home", action = "AddVisitCount", title = UrlParameter.Optional },
              new[] { "Src.Controllers" }
            );


            routes.MapRoute(
               "Page",
               "Page/{link}",
               new { controller = "Page", action = "Index" },
               new[] { "Src.Controllers" }
            );

            routes.MapRoute(
                "Search",
                "Search",
                new { controller = "Home", action = "Search" },
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
