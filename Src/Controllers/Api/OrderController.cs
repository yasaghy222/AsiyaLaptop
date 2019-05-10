using Src.Models.Service.Repository;
using Src.Models.ViewData.Base;
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

        #endregion

        #region order
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
        #endregion

    }
}
