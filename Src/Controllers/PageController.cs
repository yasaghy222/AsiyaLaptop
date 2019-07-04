using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mapster;
using System.Web.Mvc;
using Src.Models.Service.Repository;
using Src.Models.ViewData.Table;

namespace Src.Controllers
{
    public class PageController : BaseController
    {
        public PageController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        [HttpGet]
        public ActionResult Index(string link)
        {
            var Data = _unitOfWork.Page.Single(item => item.Link == link && item.Status);
            if (Data != null)
            {
                return View(model: Data.Adapt<Page.ViewPage>());
            }
            return Redirect("/");
        }
    }
}