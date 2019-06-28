using Mapster;
using Src.Models.Data;
using Src.Models.Service.Repository;
using Src.Models.Utitlity;
using Src.Models.ViewData.Base;
using Src.Models.ViewData.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Src.Controllers
{
    public class PublicAction : ActionFilterAttribute { }

    public class ValidateModel : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var viewData = filterContext.Controller.ViewData;

            if (!viewData.ModelState.IsValid)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult
                    {
                        Data = new Common.Result { Message = Common.ResultMessage.BadRequest }
                    };
                }
                else
                {
                    filterContext.Result = new ViewResult();
                    ViewResultBase contextResult = (filterContext.Result as ViewResultBase);
                    contextResult.ViewBag.Result = new Common.Result { Message = Common.ResultMessage.BadRequest };
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }

    public class BaseController : Controller
    {
        #region variable
        protected object Data;
        protected string RedirectPath;
        protected Common.Result Result;
        protected IUnitOfWork _unitOfWork;
        #endregion

        public BaseController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        #region general function
        public static List<Menu.ViewTbl_Menu> GetMenu()
        {
            using (ALDBEntities aLDB = new ALDBEntities())
            {
                var data = aLDB.Tbl_Menu.Where(item => item.Status).OrderBy(item => item.Sort).ThenBy(item => item.ID);
                return data.ToList().Adapt<List<Menu.ViewTbl_Menu>>();
            }
        }
        #endregion

        #region authorize & check action
        protected void ClearCookie()
        {
            if (ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("ALCustInfo"))
            {
                HttpCookie cookie = ControllerContext.HttpContext.Request.Cookies["ALCustInfo"];
                cookie.Expires = DateTime.Now.AddDays(-1);
                ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            }
        }
        string IsAuthorize()
        {
            if (Request.Cookies.Get("ALCustInfo") != null)
            {
                string message,
                       token = Request.Cookies["ALCustInfo"]["Token"],
                       hashToken = Function.GenerateHash(token);

                Tbl_Customer customer = _unitOfWork.Customer.Single(item => item.Token == hashToken);
                if (customer != null)
                {
                    if (customer.Status)
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
                return message;
            }
            else
            {
                return Common.ResultMessage.TokenExpire;
            }
        }
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            #region variable
            bool IsPublicAction;
            string Action, Controller, Url;
            string[] Login = { "account/logout", "account/changepass" },
                     outLogin = { "account/index", "account/resetpass" };
            #endregion

            #region get info
            Action = filterContext.RouteData.Values["Action"].ToString();
            Controller = filterContext.RouteData.Values["Controller"].ToString();
            Url = $"{Controller.ToLower()}/{Action.ToLower()}";
            RedirectPath = filterContext.Controller.ViewBag.RedirectPath?.ToString();
            IsPublicAction = filterContext.ActionDescriptor.GetCustomAttributes(typeof(PublicAction), true).Count() > 0;
            void SetResult(Common.Result result)
            {
                ViewResultBase contextResult = (filterContext.Result as ViewResultBase);
                contextResult.ViewBag.Result = result;
            }
            ActionResult GetResponse(Common.Result result, string redirectPath = null)
            {
                ActionResult actionResult;
                if (redirectPath != null)
                {
                    SetResult(result);
                    actionResult = new RedirectResult(redirectPath);
                }
                else
                {
                    #region check  is ajax request
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        actionResult = new JsonResult { Data = result };
                    }
                    else
                    {
                        SetResult(result);
                        actionResult = new ViewResult();
                    }
                    #endregion
                }
                return actionResult;
            }
            #endregion

            #region check is public
            string message = IsAuthorize();
            if (!IsPublicAction)
            {
                #region check authorize
                if (message != Common.ResultMessage.OK)
                {
                    filterContext.Result = GetResponse(Result, "/");
                }
                #endregion
            }
            #endregion

            base.OnActionExecuted(filterContext);
        }
        #endregion
    }
}