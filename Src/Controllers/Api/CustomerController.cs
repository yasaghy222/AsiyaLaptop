using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Src.Models.Service.Repository;
using Src.Models.ViewData.Base;

namespace Src.Controllers.Api
{
    public class CustomerController : BaseApiController
    {
        public CustomerController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        [HttpGet]
        public async Task<Common.Result> Get(Common.TableVar tableVar)
        {
            Data = await Task.Run(() => _unitOfWork.Customer.CustList(tableVar));
            if (Data != null)
            {
                Result.Message = Common.ResultMessage.OK;
                Result.Data = new
                {
                    List = Data,
                    Count = await _unitOfWork.Customer.GetCountAsync()
                };
            }
            else
            {
                Result.Message = Common.ResultMessage.InternallServerError;
            }
            return Result;
        }

        /// <summary>
        /// return customer total count
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<long> GetCount() => await _unitOfWork.Customer.GetCountAsync();

        [HttpPost]
        public async Task<Common.Result> ChangeStatus([FromBody]int id)
        {
            if (ModelState.IsValid)
            {
                var Data = await _unitOfWork.Customer.SingleByIdAsync(id);
                if (Data != null)
                {
                    Data.Status = Data.Status ? false : true;
                    try
                    {
                        await _unitOfWork.SaveAsync();
                        return new Common.Result { Message = Common.ResultMessage.OK };
                    }
                    catch (Exception)
                    {
                        return new Common.Result { Message = Common.ResultMessage.InternallServerError };
                    }
                }
                else
                {
                    return new Common.Result { Message = Common.ResultMessage.NotFound };
                }
            }
            else
            {
                return new Common.Result { Message = Common.ResultMessage.BadRequest };
            }
        }
    }
}
