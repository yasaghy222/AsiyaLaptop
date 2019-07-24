using Src.Models.Service.Repository;
using Src.Models.Utitlity;
using Src.Models.ViewData.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Src.Models.ViewData;
using Newtonsoft.Json;
using Src.Models.Data;
using Src.Controllers;
using static Src.App_Start.FilterConfig;

namespace Src.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        #region variable
        protected dynamic Data;
        protected string RedirectPath;
        protected Common.Result Result;
        protected HttpResponseMessage _HttpResponse;
        protected HttpClient Client = new HttpClient();
        #endregion

        #region api functions
        protected Common.Result GetResult()
        {
            Task<string> data = _HttpResponse.Content.ReadAsStringAsync();
            Result = JsonConvert.DeserializeObject<Common.Result>(data.Result.ToString());
            return Result;
        }
        protected BaseController()
        {
            #region api configure
            Client.BaseAddress = new Uri("http://localhost:4219/Api/V1/");
            Client.DefaultRequestHeaders.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            #endregion
        }
        #endregion

        #region authorize & check action
        protected void ClearCookie()
        {
            if (ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("ALAdminInfo"))
            {
                var cookie = ControllerContext.HttpContext.Request.Cookies["ALAdminInfo"];
                if (cookie != null)
                {
                    cookie.Expires = DateTime.Now.AddDays(-1);
                    ControllerContext.HttpContext.Response.Cookies.Add(cookie);
                }
            }
        }

        private string IsAuthorize()
        {
            if (Request.Cookies.Get("ALAdminInfo") != null)
            {
                string message,
                       token = Request.Cookies["ALAdminInfo"]?["Token"];
                _HttpResponse = Client.PostAsJsonAsync("Account/IsAuthorize", token).Result;
                if (_HttpResponse.IsSuccessStatusCode)
                {
                    Result = GetResult();
                    var admin = Result.Data.DeserializeJson<Tbl_Admin>();
                    if (admin != null)
                    {
                        if (admin.Status)
                        {
                            message = Common.ResultMessage.OK;
                        }
                        else
                        {
                            message = Common.ResultMessage.AccountIsBlock;
                            ClearCookie();
                        }
                    }
                    else
                    {
                        message = Common.ResultMessage.NotFound;
                        ClearCookie();
                    }
                }
                else
                {
                    message = Common.ResultMessage.InternallServerError;
                }
                return message;
            }
            else
            {
                return Common.ResultMessage.TokenExpire;
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext context)
        {
            #region get admin info
            Models.ViewData.Table.Admin.ViewAdmin viewAdmin = Function.GetAdminInfo(Request);
            if (viewAdmin != null)
            {
                ViewBag.FullName = viewAdmin.FullName;
                ViewBag.RoleName = viewAdmin.RoleName;
                ViewBag.RoleID = viewAdmin.RoleID;
                ViewBag.Token = viewAdmin.Token;
            }
            #endregion

            #region get info
            RedirectPath = context.Controller.ViewBag.RedirectPath?.ToString();
            var isPublicAction = context.ActionDescriptor.GetCustomAttributes(typeof(PublicAction), true).Any();
            void SetResult(Common.Result result)
            {
                if (result == null || context.Result == null) return;
                if (context.Result is ViewResultBase contextResult) contextResult.ViewBag.Result = result;
            }
            void GetResponse(Common.Result result, string redirectPath = null)
            {
                if (redirectPath != null)
                {
                    SetResult(result);
                    context.Result = new RedirectResult(redirectPath);
                }
                else
                {
                    #region check is ajax request
                    if (context.HttpContext.Request.IsAjaxRequest())
                    {
                        context.Result = new JsonResult { Data = result };
                    }
                    else
                    {
                        SetResult(result);
                        context.Result = new ViewResult();
                    }
                    #endregion
                }
            }
            #endregion

            #region check is public
            var message = IsAuthorize();
            if (!isPublicAction)
            {
                #region check authorize
                if (message != Common.ResultMessage.OK)
                {
                    GetResponse(new Common.Result { Message = message }, "/al-manage/login");
                }
                #endregion
            }
            else if (message == Common.ResultMessage.OK)
            {
                string[] loginAction = { "account", "account/index", "account/resetpass" };
                string controller = context.ActionDescriptor.ControllerDescriptor.ControllerName,
                    action = context.ActionDescriptor.ActionName;
                if (loginAction.Contains($"{controller.ToLower()}/{action.ToLower()}")) GetResponse(null, "/");
            }

            #endregion

            #region validate model state
            string method = context.HttpContext.Request.HttpMethod;
            if (method.ToLower() == "post")
            {
                ViewDataDictionary viewData = context.Controller.ViewData;
                if (!viewData.ModelState.IsValid)
                {
                    if (context.HttpContext.Request.IsAjaxRequest())
                    {
                        context.Result = new JsonResult
                        {
                            Data = new Common.Result { Message = Common.ResultMessage.BadRequest }
                        };
                    }
                    else
                    {
                        SetResult(new Common.Result { Message = Common.ResultMessage.BadRequest });
                        context.Result = new ViewResult();
                    }
                }
            }
            #endregion

            base.OnActionExecuting(context);
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Client != null)
                {
                    Client.Dispose();
                    Client = null;
                }
            }
        }
    }
}