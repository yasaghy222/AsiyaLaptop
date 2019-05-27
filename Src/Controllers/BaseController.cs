using Src.Models.Data;
using Src.Models.Service.Repository;
using Src.Models.Utitlity;
using Src.Models.ViewData.Base;
using System;
using System.Linq;
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

        #region authorize & check action
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
                        Request.Cookies.Remove("ALCustInfo");
                    }
                }
                else
                {
                    message = Common.ResultMessage.NotFound;
                    Request.Cookies.Remove("ALCustInfo");
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
            ActionResult GetResponse(Common.Result result, string redirectPath = null)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    if (redirectPath != null)
                    {
                        return new JsonResult
                        {
                            Data = new { result, Redirect = redirectPath }
                        };
                    }
                    else
                    {
                        return new JsonResult
                        {
                            Data = result
                        };
                    }
                }
                else
                {
                    ActionResult actionResult;
                    if (redirectPath != null)
                    {
                        actionResult = new RedirectResult(redirectPath);
                    }
                    else
                    {
                        actionResult = new ViewResult();
                    }
                    return actionResult;
                }
            }
            #endregion

            #region check is public
            string message = IsAuthorize();
            if (!IsPublicAction)
            {
                #region check authorize
                if (message != Common.ResultMessage.OK)
                {
                    #region check need login action
                    if (Login.Contains(Url))
                    {
                        filterContext.Result = GetResponse(Result, "/");
                    }
                    else
                    {
                        filterContext.Result = GetResponse(Result, RedirectPath ?? "/Account");
                    }
                    #endregion
                }
                #endregion
            }
            #endregion

            base.OnActionExecuted(filterContext);
        }
        #endregion
    }
}