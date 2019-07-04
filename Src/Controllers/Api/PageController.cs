using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Mapster;
using Src.Models.Data;
using Src.Models.Service.Repository;
using Src.Models.ViewData.Base;
using Src.Models.ViewData.Table;

namespace Src.Controllers.Api
{
    public class PageController : BaseApiController
    {
        public PageController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        #region variables
        Tbl_Page Page;
        #endregion

        #region page
        [HttpGet]
        public Common.Result Get()
        {
            Data = _unitOfWork.Page.Get();
            if (Data != null)
            {
                Result.Message = Common.ResultMessage.OK;
                Result.Data = Data.Adapt<List<Page.ViewTbl_Page>>();
            }
            else
            {
                Result.Message = Common.ResultMessage.InternallServerError;
            }
            return Result;
        }

        [HttpGet]
        public async Task<Common.Result> Detail([FromUri] int id)
        {
            if (id != -1)
            {
                #region edit
                Data = await Task.Run(() => _unitOfWork.Page.SingleById(id).Adapt<Page.ViewTbl_Page>());
                #endregion
            }
            else
            {
                #region add
                Data = new Page.ViewTbl_Page { ID = -1, Status = false };
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
        public async Task<Common.Result> AddEdit([FromBody] Page.ViewTbl_Page model)
        {
            if (ModelState.IsValid)
            {
                Page = model.Adapt<Tbl_Page>();
                if (Page.ID == -1)
                {
                    #region add
                    _unitOfWork.Page.Add(Page);
                    #endregion
                }
                else
                {
                    #region edit
                    _unitOfWork.Page.Update(Page);
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
            }
            else
            {
                Result.Message = Common.ResultMessage.BadRequest;
            }
            return Result;
        }

        [HttpPost]
        public async Task<Common.Result> Delete([FromBody]int id)
        {
            if (ModelState.IsValid)
            {
                Page = await _unitOfWork.Page.SingleByIdAsync(id);
                if (Page != null)
                {
                    _unitOfWork.Page.Remove(Page);
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
