using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Mapster;
using Src.Models.Data;
using Src.Models.Service.Repository;
using Src.Models.Utitlity;
using Src.Models.ViewData.Base;
using Src.Models.ViewData.Table;

namespace Src.Controllers.Api
{
    public class AdminController : BaseApiController
    {
        public AdminController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        #region variables
        Tbl_Admin Admin;
        #endregion

        [HttpGet]
        public Common.Result Get()
        {
            Data = _unitOfWork.Admin.Get(orderby: item => item.OrderBy(x => x.Name));
            if (Data != null)
            {
                Result.Message = Common.ResultMessage.OK;
                Result.Data = Data.Adapt<List<Admin.ViewTbl_Admin>>();
            }
            else
            {
                Result.Message = Common.ResultMessage.InternallServerError;
            }
            return Result;
        }

        [HttpGet]
        public async Task<Common.Result> Detail(int id)
        {
            if (id != -1)
            {
                #region edit
                Data = await Task.Run(() => _unitOfWork.Admin.SingleById(id).Adapt<Admin.ViewTbl_Admin>());
                #endregion
            }
            else
            {
                #region add
                Data = new Admin.ViewTbl_Admin { ID = -1 };
                #endregion
            }

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
        public async Task<Common.Result> AddEdit([FromBody] Admin.ViewTbl_Admin model)
        {
            model.Token = Function.GenerateHash(model.Token);
            Admin = model.Adapt<Tbl_Admin>();
            if (Admin.ID == -1)
            {
                #region add
                Admin.ID = 1 + Function.GenerateNumCode();
                Admin.Pass = Function.GenerateHash(Admin.NatCode);
                _unitOfWork.Admin.Add(Admin);
                #endregion
            }
            else
            {
                #region edit
                _unitOfWork.Admin.Update(Admin);
                #endregion
            }
            try
            {
                await _unitOfWork.SaveAsync();
                Result.Message = Common.ResultMessage.OK;
            }
            catch (Exception)
            {
                Result.Message = Common.ResultMessage.InternallServerError;
            }
            return Result;
        }

        [HttpPost]
        public async Task<Common.Result> Delete([FromBody]int id)
        {
            if (ModelState.IsValid)
            {
                Admin = await _unitOfWork.Admin.SingleByIdAsync(id);
                if (Admin != null)
                {
                    _unitOfWork.Admin.Remove(Admin);
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
    }
}
