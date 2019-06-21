using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Src.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        [Route("/404")]
        public ActionResult NotFound() => View();
    }
}