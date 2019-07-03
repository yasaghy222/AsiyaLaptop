using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mapster;
using Src.Models.Service.Repository;
using Src.Models.ViewData.Table;
using Src.Models.Data;
using Src.Models.ViewData.Base;
using System.Threading.Tasks;
using Src.Models.Utitlity;

namespace Src.Controllers
{
    public class NewsletterController : BaseController
    {
        public NewsletterController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        #region variables
        Tbl_Newsletter Newsletter;
        Tbl_Customer Customer;
        #endregion

        #region change customer newsletter state
        private async Task<Common.Result> ChangeNewsletter(string email)
        {
            Customer = await _unitOfWork.Customer.SingleAsync(item => item.Email == email);
            if (Customer != null)
            {
                if (Customer.IsInNewsletter == false)
                {
                    Customer.IsInNewsletter = true;
                    try
                    {
                        await _unitOfWork.SaveAsync();
                    }
                    catch (Exception)
                    {
                        return new Common.Result
                        {
                            Message = Common.ResultMessage.InternallServerError
                        };
                    }
                }
            }
            return new Common.Result
            {
                Message = Common.ResultMessage.OK
            };
        }
        #endregion

        #region register
        public async Task<Common.Result> Register(Newsletter.ViewTbl_Newsletter newsletter)
        {
            Data = await _unitOfWork.Newsletter.SingleAnyAsync(item => item.Email == newsletter.Email);
            if (!Data)
            {
                Newsletter = new Tbl_Newsletter
                {
                    Email = newsletter.Email,
                    IP = Function.GetIP()
                };
                _unitOfWork.Newsletter.Add(Newsletter);
                try
                {
                    await _unitOfWork.SaveAsync();
                    return await ChangeNewsletter(Newsletter.Email);
                }
                catch (Exception)
                {
                    return new Common.Result
                    {
                        Message = Common.ResultMessage.InternallServerError
                    };
                }
            }
            else
            {
                return new Common.Result
                {
                    Message = "شما قبلا در خبرنامه ثبت نام کرداید"
                };
            }
        }
    }
    #endregion
}