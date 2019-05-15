using Src.Models.ViewData.Base;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Src.App_Start
{

    public class Public : ActionFilterAttribute { }

    public class Account : ActionFilterAttribute
    {
        #region variable
        object Data;
        bool TempBool = false;
        Common.Resualt resualt;
        string Controller, Action, Token;
        #endregion

        #region authentication functions
        //get user info
        private class ContextInfo
        {
            #region variables
            private HttpActionExecutedContext _context;
            private HttpActionDescriptor descriptor;
            #endregion

            #region functions
            public ContextInfo(HttpActionExecutedContext context)
            {
                _context = context;
                descriptor = context.Request.GetActionDescriptor();
            }
            public bool IsPublicAction() => descriptor.GetCustomAttributes<ContextInfo>(true).Count() > 0;
            public string GetAction() => _context.ActionContext.ControllerContext.RouteData.Values["Action"].ToString();
            public string GetController() => _context.ActionContext.ControllerContext.RouteData.Values["Controller"].ToString();
            public string GetToken() => _context.ActionContext.ControllerContext.RouteData.Values["Token"].ToString();
            #endregion
        }

        //check is user login or not
        private Common.Resualt CheckToken(string token)
        {
            #region check token
            TempBool = true;
            #endregion

            if (TempBool)
            {
                resualt.Message = Common.ResualtMessage.OK;
                resualt.Data = 12;
            }
            else
            {
                resualt.Message = Common.ResualtMessage.NotFound;
            }
            return resualt;
        }

        //check action when user is login and not login
        private bool CheckAction(string action, bool isLogin)
        {
            action = action.ToLower();
            string[] Login = { "logout", "changepass" };
            string[] outLogin = { "index", "login", "register", "resetpass" };

            if (isLogin)
            {
                return outLogin.Contains(action) ? false : true;
            }
            else
            {
                return Login.Contains(action) ? false : true;
            }
        }

        //check is user have permission to this action 
        private Common.Resualt IsAuthoriez(string token, string controller, string action)
        {
            // check login
            resualt = CheckToken(token);
            TempBool = resualt.Message == Common.ResualtMessage.OK ? true : false;

            if (CheckAction(action, TempBool))
            {
                #region check user role
                int roleId = (int)resualt.Data;
                #endregion
                return resualt;
            }
            else
            {
                resualt.Message = Common.ResualtMessage.BadRequest;
                return resualt;
            }
        }

        //authenticate user
        private Common.Resualt IsAuthenticate(HttpActionExecutedContext context)
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
                // check user is authorize
                return IsAuthoriez(Token, Controller, Action);
            }

            return resualt;
        }
        #endregion

        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            resualt = IsAuthenticate(context);
            if (resualt.Message == Common.ResualtMessage.OK)
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
            else
            {
                Data = resualt;
            }

            context.Response = context.Request.CreateResponse(Data);
            base.OnActionExecuted(context);
        }
    }
}