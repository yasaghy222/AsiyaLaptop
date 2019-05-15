using Src.Models.Service.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Src.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        [HttpGet]
        public ActionResult Index() => View();

        [HttpGet]
        public ActionResult ResetPass() => View();
    }
}