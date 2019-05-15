using System;
using System.Collections.Generic;
using System.Linq;
using Src.Models.Utitlity;
using Src.Models.ViewData.Base;
using Src.Models.ViewData.Table;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.IO;
using Mapster;
using Newtonsoft.Json;

namespace Src.Areas.Admin.Controllers
{
    public class OrderController : BaseController
    {
        [HttpGet]
        public ActionResult Index() => View();
    }
}
