using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Mapster;
using Src.Models.Data;
using Src.Models.Service.Repository;
using Src.Models.Utitlity;
using Src.Models.ViewData.Base;
using Src.Models.ViewData.Table;
using static Src.App_Start.FilterConfig;

namespace Src.Controllers.Api
{
    public class AccountController : BaseApiController
    {
        public AccountController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        #region variables
        Tbl_Admin Admin;
        #endregion

        #region account authorize ajax
        [HttpPost, PublicAction]
        public Common.Result IsAuthorize([FromBody] string token)
        {
            string hashToken = Function.GenerateHash(token);
            Tbl_Admin admin = _unitOfWork.Admin.Single(item => item.Token == hashToken && item.Status);
            if (admin != null)
            {
                if (admin.Status)
                {
                    return Result = new Common.Result
                    {
                        Message = Common.ResultMessage.OK,
                        Data = admin.Adapt<Admin.ViewTbl_Admin>()
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

        private async Task<Common.Result> IsValid(Tbl_Admin admin, Admin.LoginVar loginVar)
        {
            string hashPass = Function.GenerateHash(loginVar.Pass);
            if (admin.Pass == hashPass)
            {
                if (admin.Status)
                {
                    string token = Function.GenerateNewToken();
                    admin.Token = Function.GenerateHash(token);
                    Admin.ViewAdmin info = admin.Adapt<Admin.ViewAdmin>();
                    info.Token = token;
                    Result.Data = info;
                    Result.Message = Common.ResultMessage.OK;
                }
                else
                {
                    Result.Message = Common.ResultMessage.AccountIsBlock;
                }
            }
            else
            {
                if (admin.LFCount < 5)
                {
                    admin.LFCount++;
                }
                else
                {
                    admin.LFCount = 0;
                    admin.Status = false;
                }
                Result.Message = "نام کاربری یا رمز عبور اشتباه است.";
            }

            try
            {
                await _unitOfWork.SaveAsync();
            }
            catch
            {
                Result.Message = Common.ResultMessage.InternallServerError;
            }
            return Result;
        }
        #endregion

        [HttpPost, PublicAction]
        public async Task<Common.Result> Profile([FromBody] string token)
        {
            string hashToken = Function.GenerateHash(token);
            Admin = await _unitOfWork.Admin.SingleAsync(item => item.Token == hashToken);
            if (Admin != null)
            {
                Result.Data = Admin.Adapt<Admin.ViewTbl_Admin>();
                Result.Message = Common.ResultMessage.OK;
            }
            else
            {
                Result.Message = Common.ResultMessage.NotFound;
            }
            return Result;
        }

        [HttpPost, PublicAction]
        public async Task<Common.Result> Login([FromBody] Admin.LoginVar loginVar)
        {
            Admin = await _unitOfWork.Admin.SingleAsync(item => item.Phone == loginVar.Phone);
            if (Admin != null)
            {
                Result = await IsValid(Admin, loginVar);
            }
            else
            {
                Result.Message = Common.ResultMessage.NotFound;
            }
            return Result;
        }

        [HttpPost]
        public async Task<Common.Result> Logout([FromBody] string token)
        {
            string hashToken = Function.GenerateHash(token);
            Admin = await _unitOfWork.Admin.SingleAsync(item => item.Token == hashToken);
            if (Admin != null)
            {
                Admin.Token = "";
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
            return Result;
        }

        [HttpPost]
        public async Task<Common.Result> ChangePass([FromBody] Admin.ChangePassVar changePassVar, [FromUri] string token)
        {
            string hashToken = Function.GenerateHash(token),
                   hashOldPass = Function.GenerateHash(changePassVar.OldPass);
            Admin = await _unitOfWork.Admin.SingleAsync(item => item.Token == hashToken && item.Pass == hashOldPass);
            if (Admin != null)
            {
                Admin.Pass = Function.GenerateHash(changePassVar.NewPass);
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
            return Result;
        }
    }
}
