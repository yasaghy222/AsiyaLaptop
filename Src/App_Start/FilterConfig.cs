using Src.Controllers;
using Src.Controllers.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Src.App_Start
{
    public class FilterConfig
    {
        public class PublicAction : ActionFilterAttribute { }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
        }
    }
}