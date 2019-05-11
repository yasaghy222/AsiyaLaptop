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
        public async Task<Common.Resualt> Get([FromUri]Common.TableVar tableVar)
        {
            Data = _unitOfWork.Factor.OrderList(tableVar);
            if (Data != null)
            {
                Resualt.Message = Common.ResualtMessage.OK;
                Resualt.Data = new
                {
                    List = Data,
                    Count = await _unitOfWork.Factor.GetCountAsync()
                };
            }
            else
            {
                Resualt.Message = Common.ResualtMessage.InternallServerError;
            }
            return Resualt;
        }

        [HttpGet]
        public Common.Resualt Detail([FromUri] int id)
        {
            Data = _unitOfWork.Factor.SingleById(id).Adapt<Factor.ViewOrderDetail>();

            if (Data != null)
            {
                Resualt.Message = Common.ResualtMessage.OK;
                Resualt.Data = Data;
            }
            else
            {
                Resualt.Message = Common.ResualtMessage.InternallServerError;
            }
            return Resualt;
        }

        [HttpPost]
        public async Task<Common.Resualt> ChangeStatus([FromBody] int id, [FromBody] byte status)
        {
            if (ModelState.IsValid)
            {
                Factor = await _unitOfWork.Factor.SingleByIdAsync(id);
                if (Factor != null)
                {
                    Factor.Status = status;
                    try
                    {
                        await _unitOfWork.SaveAsync();
                        Resualt.Message = Common.ResualtMessage.OK;
                    }
                    catch (Exception)
                    {
                        Resualt.Message = Common.ResualtMessage.InternallServerError;
                    }
                }
                else
                {
                    Resualt.Message = Common.ResualtMessage.NotFound;
                }
            }
            else
            {
                Resualt.Message = Common.ResualtMessage.BadRequest;
            }
            return Resualt;
        }
        #endregion
    }
}
