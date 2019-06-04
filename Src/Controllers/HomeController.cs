using Mapster;
using Src.Models.Data;
using Src.Models.Service.Repository;
using Src.Models.ViewData.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Src.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        [HttpGet, PublicAction]
        public ActionResult Index() => View();
    }
}