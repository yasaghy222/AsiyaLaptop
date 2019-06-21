using Mapster;
using Src.Models.Service.Repository;
using Src.Models.ViewData.Base;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Src.Controllers.Api
{
    public class PublicHttpAction : ActionFilterAttribute { }

    public class ValidateModel : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            if (!filterContext.ModelState.IsValid)
            {
                using (_ = filterContext.Request.CreateResponse(new Common.Result { Data = null, Message = Common.ResultMessage.BadRequest }))
                {
                    base.OnActionExecuting(filterContext);
                }
            }
        }
    }


    public class BaseApiController : ApiController
    {
        #region variable
        protected object Data;
        protected IUnitOfWork _unitOfWork;
        protected NameValueCollection FormData;
        protected Common.Result Result = new Common.Result();
        #endregion

        public BaseApiController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        #region authorize & authenticate
        Common.Result IsAuthorize(string token)
        {
            bool isExsit = true;
            if (isExsit)
            {
                bool status = true;
                if (status)
                {
                    return Result = new Common.Result
                    {
                        Message = Common.ResultMessage.OK,
                        Data = 12
                    };
                }
                else
                {
                    return Result = new Common.Result
                    {
                        Message = Common.ResultMessage.AccountIsBlock,
                    };
                }
            }
            else
            {
                return Result = new Common.Result
                {
                    Message = Common.ResultMessage.NotFound
                };
            }
        }
        Common.Result HasPermisstion(int roleID, string action, string controller) =>
        Result = new Common.Result
        {
            Message = Common.ResultMessage.OK
        };
        public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
        {
            #region variable
            bool IsPublicAction;
            string Token, Action, Controller;
            #endregion

            #region get info
            Token = controllerContext.RouteData.Values["Token"].ToString();
            Action = controllerContext.RouteData.Values["Action"].ToString();
            Controller = controllerContext.RouteData.Values["Controller"].ToString();
            IsPublicAction = controllerContext.ControllerDescriptor.GetCustomAttributes<BaseApiController>(true).Count > 0;
            object GetResponse()
            {
                if (controllerContext.Request.Content != null)
                {
                    Task<string> Temp = controllerContext.Request.Content.ReadAsStringAsync();
                    string tempResult = Temp.Result.ToString();
                    return tempResult != "" ? System.Web.Helpers.Json.Decode(Temp.Result.ToString()) : Result;
                }
                else
                {
                    return Result = new Common.Result
                    {
                        Message = Common.ResultMessage.InternallServerError
                    };
                }
            }
            #endregion

            #region check is public
            if (IsPublicAction)
            {
                Data = GetResponse();
            }
            else
            {
                #region authorize
                Result = IsAuthorize(Token);
                if (Result.Message == Common.ResultMessage.OK)
                {
                    #region check permission
                    Result = HasPermisstion((int)Result.Data, Action, Controller);
                    Data = Result.Message == Common.ResultMessage.OK ? GetResponse() : Result;
                    #endregion
                }
                else
                {
                    Data = Result;
                }
                #endregion
            }
            #endregion

            using (_ = controllerContext.Request.CreateResponse(Data))
            {
                return base.ExecuteAsync(controllerContext, cancellationToken);
            }
        }
        #endregion

        #region common function
        /// <summary>
        /// return list of product brands
        /// </summary>
        /// <returns></returns>
        protected async Task<ICollection<Common.Select>> GetBrandList() => await Task.Run(() => _unitOfWork.ProcBrand.Get().Adapt<ICollection<Common.Select>>());

        /// <summary>
        /// return list of product cat
        /// </summary>
        /// <returns></returns>
        protected async Task<ICollection<Common.Tree>> GetCatList() => await Task.Run(() => _unitOfWork.ProcCat.Get().Adapt<ICollection<Common.Tree>>());
        #endregion
    }
}
