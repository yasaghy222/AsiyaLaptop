using Mapster;
using Src.Models.Data;
using Src.Models.Service.Repository;
using Src.Models.ViewData.Base;
using Src.Models.ViewData.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Src.Controllers.Api
{
    public class OrderController : BaseApiController
    {
        public OrderController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        #region variable
        Tbl_Factor Factor = null;
        #endregion

        #region order
        [HttpGet]
        public async Task<Common.Result> Get([FromUri]Common.TableVar tableVar)
        {
            Data = _unitOfWork.Factor.OrderList(tableVar);
            if (Data != null)
            {
                Result.Message = Common.ResultMessage.OK;
                Result.Data = new
                {
                    List = Data,
                    Count = await _unitOfWork.Factor.GetCountAsync()
                };
            }
            else
            {
                Result.Message = Common.ResultMessage.InternallServerError;
            }
            return Result;
        }

        [HttpGet]
        public Common.Result Detail([FromUri] int id)
        {
            Data = _unitOfWork.Factor.SingleById(id).Adapt<Factor.ViewOrderDetail>();

            if (Data != null)
            {
                Result.Message = Common.ResultMessage.OK;
                Result.Data = Data;
            }
            else
            {
                Result.Message = Common.ResultMessage.InternallServerError;
            }
            return Result;
        }

        [HttpPost]
        public async Task<Common.Result> ChangeStatus([FromBody] int id, [FromBody] byte newState)
        {
            if (ModelState.IsValid)
            {
                Factor = await _unitOfWork.Factor.SingleByIdAsync(id);
                if (Factor != null)
                {
                    Factor.Status = newState;
                    try
                    {
                        await _unitOfWork.SaveAsync();
                        Result.Message = Common.ResultMessage.OK;
                    }
                    catch (Exception)
                    {
                        Result.Message = Common.ResultMessage.InternallServerError;
                    }
                }
                else
                {
                    Result.Message = Common.ResultMessage.NotFound;
                }
            }
            else
            {
                Result.Message = Common.ResultMessage.BadRequest;
            }
            return Result;
        }
        #endregion
    }
}
