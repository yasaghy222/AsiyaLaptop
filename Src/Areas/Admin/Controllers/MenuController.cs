using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Src.Areas.Admin.Controllers
{
    public class MenuController : BaseController
    {
        [HttpGet]
        public ActionResult Index() => View();
    }
}