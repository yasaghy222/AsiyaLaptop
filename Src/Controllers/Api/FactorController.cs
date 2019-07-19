using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Mapster;
using Src.Models.Service.Repository;
using Src.Models.Utitlity;
using Src.Models.ViewData.Table;

namespace Src.Controllers.Api
{
    public class FactorController : BaseApiController
    {
        public FactorController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        [HttpGet]
        public List<Factor.ViewOrder> GetTopOrder() => _unitOfWork.Factor.Get().Take(5).Adapt<List<Factor.ViewOrder>>();

        /// <summary>
        /// return order total count and new count
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<long[]> GetOrderCount()
        {
            var TotalCount = await _unitOfWork.Factor
                                    .GetCountAsync(item => item.PaymentStatus &&
                                                   item.Status == (byte)Factor.FactStatus.DeliveryToCust);
            var NewCount = await _unitOfWork.Factor
                                             .GetCountAsync(item => item.Status == (byte)Factor.FactStatus.Registered);

            return new[] { NewCount, TotalCount };
        }

        /// <summary>
        /// دریافت میزان درآمد امروز
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<string[]> GetIncome()
        {
            var TodayIncome = await Task.Run(() => _unitOfWork.Factor
                                                               .Get(item => item.SubmitDate == DateTime.Now &&
                                                                    item.Status == (byte)Factor.FactStatus.DeliveryToCust)
                                                               .Sum(item => item.TotalPrice));
            var yesterday = DateTime.Now.AddDays(-1);
            var YesterdayIncome = await Task.Run(() => _unitOfWork.Factor
                                                              .Get(item => item.SubmitDate == yesterday &&
                                                                   item.Status == (byte)Factor.FactStatus.DeliveryToCust)
                                                              .Sum(item => item.TotalPrice));

            return new[] { TodayIncome.ToCurrency(), YesterdayIncome.ToCurrency() };
        }
    }
}
