using Src.Models.Data;
using Src.Models.Utitlity;
using Src.Models.ViewData.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Src.Areas.Admin.Controllers
{
    public class AdminController : BaseController
    {

        #region variables
        Models.ViewData.Table.Admin.ViewTbl_Admin Admin;
        #endregion

        [HttpGet]
        public ActionResult Index() => View();

        [HttpGet]
        public ActionResult AddEdit(int? id = -1)
        {
            _HttpResponse = Client.GetAsync($"Admin/Detail/{Function.GetAdminInfo(Request).Token}?id={id}").Result;
            if (_HttpResponse.IsSuccessStatusCode)
            {
                Result = GetResult();
                Admin = Result.Data.DeserializeJson<Models.ViewData.Table.Admin.ViewTbl_Admin>();
            }
            else
            {
                ViewBag.Message = Common.ResultMessage.InternallServerError;
                return Redirect("/Admin/Admin");
            }

            return View(model: Admin);
        }

    }
}