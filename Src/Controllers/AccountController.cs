﻿using Mapster;
using Src.Models.Data;
using Src.Models.Service.Repository;
using Src.Models.Utitlity;
using Src.Models.ViewData.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static Src.Models.ViewData.Table.Customer;

namespace Src.Controllers
{
    public class AccountController : BaseController
    {
        #region variable
        Tbl_Customer Customer = null;
        #endregion

        public AccountController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        [HttpGet]
        public ActionResult Index() => View();

        [HttpGet]
        public ActionResult ResetPass() => View();

        #region authorize functions
        void SetCookie(CustomerInfo info)
        {
            HttpCookie cookie = new HttpCookie("ALCustInfo")
            {
                Expires = DateTime.Now.AddMonths(3)
            };
            cookie.Values.Add("Token", info.Token);
            cookie.Values.Add("Name", info.Name);
            cookie.Values.Add("Family", info.Family);
            HttpContext.Response.SetCookie(cookie);
        }
        async Task<bool> SetToken(Tbl_Customer customer)
        {
            customer.Token = Function.GenerateNewToken();
            CustomerInfo info = customer.Adapt<CustomerInfo>();
            customer.Token = Function.GenerateHash(customer.Token);
            await _unitOfWork.SaveAsync();
            try
            {
                SetCookie(info);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        async Task<string> IsValid(ViewLoginVar loginVar)
        {
            string hashPass = Function.GenerateHash(loginVar.Pass);
            Customer = await _unitOfWork.Customer.SingleAsync(item =>
                                                                    item.Phone == loginVar.Phone &&
                                                                    item.Pass == hashPass);
            #region check status
            if (Customer.Status)
            {
                return await SetToken(Customer) ? Common.ResualtMessage.OK : Common.ResualtMessage.InternallServerError;
            }
            else
            {
                return Common.ResualtMessage.AccountIsBlock;
            }
            #endregion
        }
        #endregion

        #region login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(ViewLoginVar loginVar)
        {
            if (ModelState.IsValid)
            {
                string hashPass = Function.GenerateHash(loginVar.Pass);
                bool isExist = await _unitOfWork.Customer.SingleAnyAsync(item =>
                                                                         item.Phone == loginVar.Phone &&
                                                                         item.Pass == hashPass);
                Resualt.Message = isExist ? await IsValid(loginVar) : Common.ResualtMessage.NotFound;
            }
            else
            {
                Resualt.Message = Common.ResualtMessage.BadRequest;
            }

            ViewBag.Resualt = Resualt;
            return View();
        }
        #endregion

        #region register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register()
        {
            return Redirect("/");
        }
        #endregion
    }
}