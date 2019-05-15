using Mapster;
using Src.App_Start;
using Src.Models.Service.Repository;
using Src.Models.ViewData.Base;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using static Src.Models.ViewData.Table.Admin;

namespace Src.Controllers.Api
{
    public class BaseApiController : ApiController
    {
        #region variable
        protected object Data;
        protected IUnitOfWork _unitOfWork;
        protected NameValueCollection FormData;
        protected Common.Resualt Resualt = new Common.Resualt();
        #endregion

        public BaseApiController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        #region authorize & authenticate
        Common.Resualt IsAuthorize(string token)
        {
            bool isExsit = true;
            if (isExsit)
            {
                bool status = true;
                if (status)
                {
                    return Resualt = new Common.Resualt
                    {
                        Message = Common.ResualtMessage.OK,
                        Data = 12
                    };
                }
                else
                {
                    return Resualt = new Common.Resualt
                    {
                        Message = Common.ResualtMessage.AccountIsBlock,
                    };
                }
            }
            else
            {
                return Resualt = new Common.Resualt
                {
                    Message = Common.ResualtMessage.NotFound
                };
            }
        }
        Common.Resualt HasPermisstion(int roleID, string action, string controller)
        {
            return Resualt = new Common.Resualt
            {
                Message = Common.ResualtMessage.OK
            };
        }
        public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext,CancellationToken cancellationToken)
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
                    return System.Web.Helpers.Json.Decode(Temp.Result.ToString());
                }
                else
                {
                    return Resualt = new Common.Resualt
                    {
                        Message = Common.ResualtMessage.InternallServerError
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
                Resualt = IsAuthorize(Token);
                if (Resualt.Message == Common.ResualtMessage.OK)
                {
                    #region check permission
                    Resualt = HasPermisstion((int)Resualt.Data, Action, Controller);
                    Data = Resualt.Message == Common.ResualtMessage.OK ? GetResponse() : Resualt;
                    #endregion
                }
                else
                {
                    Data = Resualt;
                }
                #endregion
            }
            #endregion

            controllerContext.Request.CreateResponse(Data);
            return base.ExecuteAsync(controllerContext, cancellationToken);
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
