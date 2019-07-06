using Src.Controllers;
using Src.Models.Data;
using Src.Models.Utitlity;
using Src.Models.ViewData.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static Src.App_Start.FilterConfig;

namespace Src.Areas.Admin.Controllers
{
    public class AccountController : BaseController
    {

        #region variables

        #endregion

        #region functions
        private void SetCookie(Models.ViewData.Table.Admin.ViewAdmin viewAdmin)
        {
            HttpCookie cookie = new HttpCookie("ALAdminInfo")
            {
                Expires = DateTime.Now.AddMonths(6)
            };
            cookie.Values.Add("Token", viewAdmin.Token);
            cookie.Values.Add("FullName", viewAdmin.FullName);
            cookie.Values.Add("RoleName", viewAdmin.RoleName);
            cookie.Values.Add("RoleID", viewAdmin.RoleID.ToString());
            HttpContext.Response.SetCookie(cookie);
        }
        #endregion

        [HttpGet]
        public ActionResult Index() => View(new Models.ViewData.Table.Admin.ViewTbl_Admin());

        [HttpGet]
        public ActionResult ChangePass() => View();

        [HttpGet, PublicAction]
        public ActionResult Login() => View(new Models.ViewData.Table.Admin.LoginVar());

        [HttpPost, PublicAction]
        public async Task<ActionResult> Login(Models.ViewData.Table.Admin.LoginVar loginInfo)
        {
            if (ModelState.IsValid)
            {
                _HttpResponse = await Client.PostAsJsonAsync("Account/Login", loginInfo);
                if (_HttpResponse.IsSuccessStatusCode)
                {
                    Result = GetResult();
                    if (Result.Message == Common.ResultMessage.OK)
                    {
                        SetCookie(Result.Data.DeserializeJson<Models.ViewData.Table.Admin.ViewAdmin>());
                        return RedirectToRoute("al-manage");
                    }
                    else
                    {
                        ViewBag.Result = Result.Message;
                    }
                }
                else
                {
                    ViewBag.Result = Common.ResultMessage.InternallServerError;
                }
            }
            else
            {
                ViewBag.Result = Common.ResultMessage.BadRequest;
            }

            return View();
        }

        [HttpGet, PublicAction]
        public ActionResult ResetPass() => View();

        [HttpGet, PublicAction]
        public async Task<ActionResult> Logout()
        {
            Models.ViewData.Table.Admin.ViewAdmin info = Function.GetAdminInfo(Request);
            if (info != null)
            {
                _HttpResponse = await Client.PostAsJsonAsync("Account/Logout", info.Token);
                if (_HttpResponse.IsSuccessStatusCode)
                {
                    Result = GetResult();
                    if (Result.Message == Common.ResultMessage.OK)
                    {
                        ClearCookie();
                    }
                }
            }
            return RedirectToRoute("al-manage/login");
        }
    }
}