using Src.Models.Service.Repository;
using Src.Models.ViewData.Base;
using System.Linq;
using System.Web.Mvc;

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
                    contextResult.ViewBag.Resualt = new Common.Result { Message = Common.ResultMessage.BadRequest };
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
            //if (Request.Cookies.Get("ALCustInfo") != null)
            //{
            //    string token = Request.Cookies["ALCustInfo"]["Token"];
            //    Tbl_Customer customer = null;
            //    if (customer != null)
            //    {
            //        customer.Status = true;
            //        if (customer.Status)
            //        {
            //            return Resualt = new Common.Resualt
            //            {
            //                Message = Common.ResualtMessage.OK,
            //            };
            //        }
            //        else
            //        {
            //            return Resualt = new Common.Resualt
            //            {
            //                Message = Common.ResualtMessage.AccountIsBlock,
            //            };
            //        }
            //    }
            //    else
            //    {
            //        return Resualt = new Common.Resualt
            //        {
            //            Message = Common.ResualtMessage.NotFound
            //        };
            //    }
            //}
            //else
            //{
            //    return Resualt = new Common.Resualt
            //    {
            //        Message = Common.ResualtMessage.TokenExpire
            //    };
            //}
            return Common.ResultMessage.NotFound;
        }
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            #region variable
            bool IsPublicAction;
            string Action, Controller;
            string[] Login = { "logout", "changepass" },
                     outLogin = { "index", "resetpass" };
            #endregion

            #region get info
            Action = filterContext.RouteData.Values["Action"].ToString();
            Controller = filterContext.RouteData.Values["Controller"].ToString();
            RedirectPath = filterContext.Controller.ViewData["RedirectPath"]?.ToString();
            IsPublicAction = filterContext.ActionDescriptor.GetCustomAttributes(typeof(PublicAction), true).Count() > 0;
            ActionResult GetResponse(Common.Result resualt, string redirectPath = null)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    if (redirectPath != null)
                    {
                        return new JsonResult
                        {
                            Data = new { resualt, Redirect = redirectPath }
                        };
                    }
                    else
                    {
                        return new JsonResult
                        {
                            Data = resualt
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

                    ViewResultBase contextResult = (filterContext.Result as ViewResultBase);
                    contextResult.ViewBag.Resualt = resualt;
                    return actionResult;
                }
            }
            #endregion

            #region check is public
            if (!IsPublicAction)
            {
                #region check authorize
                if (IsAuthorize() != Common.ResultMessage.OK && !outLogin.Contains(Action))
                {
                    filterContext.Result = GetResponse(Result, "/Account");
                }
                else if (Result.Message == Common.ResultMessage.OK && !Login.Contains(Action))
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