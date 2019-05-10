using Src.Models.ViewData.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Src.Models.Utitlity;

namespace Src.App_Start
{

    public class Public : ActionFilterAttribute { }
    public class Account : ActionFilterAttribute
    {
        #region variable
        string Controller, Action, Token, Temp;
        object Data;
        Common.Resualt resualt;
        bool TempBool = false;
        #endregion

        private class ContextInfo
        {
            private HttpActionExecutedContext _context;
            private HttpActionDescriptor descriptor;

            public ContextInfo(HttpActionExecutedContext context)
            {
                _context = context;
                descriptor = context.Request.GetActionDescriptor();
            }

            public bool IsPublicAction() => descriptor.GetCustomAttributes<ContextInfo>(true).Count() > 0;
            public string GetAction() => _context.ActionContext.ControllerContext.RouteData.Values["Action"].ToString();
            public string GetController() => _context.ActionContext.ControllerContext.RouteData.Values["Controller"].ToString();
            public string GetToken() => _context.ActionContext.ControllerContext.RouteData.Values["Token"].ToString();
        }

        private bool CheckToken(string token)
        {
            TempBool = true;
            return TempBool;
        }

        private Common.Resualt IsAuthorize(HttpActionExecutedContext context)
        {
            ContextInfo contextInfo = new ContextInfo(context);
            Action = contextInfo.GetAction();
            Controller = contextInfo.GetController();
            Token = contextInfo.GetToken();
            resualt = new Common.Resualt();

            if (contextInfo.IsPublicAction())
            {
                resualt.Message = Common.ResualtMessage.OK;
            }
            else
            {
                #region check token
                if (CheckToken(Token))
                {
                    resualt.Message = Common.ResualtMessage.OK;
                }
                else
                {
                    resualt.Message = Common.ResualtMessage.NotFound;
                }
                #endregion
            }

            return resualt;
        }

        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            resualt = IsAuthorize(context);

            if (resualt.Message != Common.ResualtMessage.OK)
            {
                Data = resualt;
            }
            else
            {
                if (context.Response != null)
                {
                    Task<string> Temp = context.Response.Content.ReadAsStringAsync();
                    Data = Json.Decode(Temp.Result.ToString());
                }
                else
                {
                    resualt.Message = Common.ResualtMessage.InternallServerError;
                    Data = resualt;
                }
            }

            context.Response = context.Request.CreateResponse(Data);
            base.OnActionExecuted(context);
        }
    }
}