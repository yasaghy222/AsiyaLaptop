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

        protected override void OnActionExecuting(ActionExecutingContext context)
        {
            #region get info
            RedirectPath = context.Controller.ViewBag.RedirectPath?.ToString();
            bool IsPublicAction = context.ActionDescriptor.GetCustomAttributes(typeof(PublicAction), true).Count() > 0;
            void SetResult(Common.Result result = null)
            {
                if (result != null)
                {
                    ViewResultBase contextResult = (context.Result as ViewResultBase);
                    contextResult.ViewBag.Result = result;
                }
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
                    if (context.HttpContext.Request.IsAjaxRequest())
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
                    context.Result = GetResponse(Result, "/");
                }
                #endregion
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
    }
}