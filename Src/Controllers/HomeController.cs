using Src.Models.Service.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Src.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index() => View();
    }
}