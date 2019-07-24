using Mapster;
using Src.Models.Data;
using Src.Models.Service.Repository;
using Src.Models.Utitlity;
using Src.Models.ViewData.Base;
using Src.Models.ViewData.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using static Src.App_Start.FilterConfig;

namespace Src.Controllers
{
    public class BaseController : Controller
    {
        #region variable
        protected dynamic Data;
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
        public static List<Media.ViewTbl_Media> GetMedia()
        {
            using (ALDBEntities aLDB = new ALDBEntities())
            {
                var data = aLDB.Tbl_Media.OrderBy(item => item.Sort).ThenBy(item => item.ID);
                return data.ToList().Adapt<List<Media.ViewTbl_Media>>();
            }
        }
        #endregion

        #region authorize & check action
        protected void ClearCookie()
        {
            if (ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("ALCustInfo"))
            {
                var cookie = ControllerContext.HttpContext.Request.Cookies["ALCustInfo"];
                cookie.Expires = DateTime.Now.AddDays(-1);
                ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            }
        }
        private string IsAuthorize()
        {
            if (Request.Cookies.Get("ALCustInfo") == null) return Common.ResultMessage.TokenExpire;
            string message,
                token = Request.Cookies["ALCustInfo"]?["Token"],
                hashToken = Function.GenerateHash(token);
            var customer = _unitOfWork.Customer.Single(item => item.Token == hashToken);
            if (customer != null)
            {
                if (customer.Status)
                    message = Common.ResultMessage.OK;
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

        #region action context function
        private static ActionExecutingContext SetResult(ActionExecutingContext context, Common.Result result = null)
        {
            if (result == null) return context;
            var contextResult = (ViewResultBase)context.Result;
            if (contextResult != null) contextResult.ViewBag.Result = result;
            return context;
        }
        private static ActionExecutingContext SetResponse(ActionExecutingContext context, Common.Result result, string redirectPath = null)
        {
            #region set redirect
            if (redirectPath != null)
            {
                context.Result = new RedirectResult(redirectPath);
                return context;
            }
            #endregion

            #region set response
            if (context.HttpContext.Request.IsAjaxRequest())
            {
                context.Result = new JsonResult { Data = result };
            }
            else
            {
                context.ActionParameters.TryGetValue("model", out var model);
                context.Result = new ViewResult
                {
                    ViewData = new ViewDataDictionary
                    {
                        Model = model
                    }
                };
                context = SetResult(context, result);
            }
            #endregion

            return context;
        }
        #endregion

        protected override void OnActionExecuting(ActionExecutingContext context)
        {
            #region get info
            string[] loginAction = { "account", "account/index", "account/resetpass" };
            string controller = context.ActionDescriptor.ControllerDescriptor.ControllerName,
                   action = context.ActionDescriptor.ActionName,
                   route = $"{controller.ToLower()}/{action.ToLower()}";
            var isPublicAction = context.ActionDescriptor.GetCustomAttributes(typeof(PublicAction), true).Any();
            #endregion

            #region check is public
            var message = IsAuthorize();
            if (!isPublicAction)
            {
                #region check authorize
                if (message != Common.ResultMessage.OK)
                    context = SetResponse(context, new Common.Result(message), "/Account");
                #endregion
            }
            else if (message == Common.ResultMessage.OK && loginAction.Contains(route))
                context = SetResponse(context, null, "/");
            #endregion

            #region validate model state
            var method = context.HttpContext.Request.HttpMethod;
            var viewData = context.Controller.ViewData;
            if (method.ToLower() == "post" && !viewData.ModelState.IsValid)
                context = SetResponse(context, new Common.Result(Common.ResultMessage.BadRequest));
            #endregion
            base.OnActionExecuting(context);
        }
        #endregion
    }
}