using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using Src.Controllers;
using Src.Models.Data;
using Src.Models.ViewData.Table;
using Src.Models.Utitlity;
using System.ComponentModel.DataAnnotations;
using Src.Models.ViewData.Base;
using System.Linq;
using Mapster;
using Src.App_Start;

namespace Src
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ControllerBuilder.Current.SetControllerFactory(new NinjectController());
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            MapsterConfig.RegisterMap();
        }
    }
}