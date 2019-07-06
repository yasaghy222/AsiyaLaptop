using Src.Models.Data;
using Src.Models.Utitlity;
using Src.Models.ViewData.Base;
using Src.Models.ViewData.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Src.Areas.Admin.Controllers
{
    public class PageController : BaseController
    {

        #region variables
        Page.ViewTbl_Page Page;
        #endregion

        [HttpGet]
        public ActionResult Index() => View();

        [HttpGet]
        public ActionResult AddEdit(int? id = -1)
        {
            _HttpResponse = Client.GetAsync($"Page/Detail/{Function.GetAdminInfo(Request).Token}?id={id}").Result;
            if (_HttpResponse.IsSuccessStatusCode)
            {
                Result = GetResult();
                Page = Result.Data.DeserializeJson<Page.ViewTbl_Page>();
            }
            else
            {
                ViewBag.Message = Common.ResultMessage.InternallServerError;
                return Redirect("/Admin/Product");
            }

            return View(model: Page);
        }
    }
}