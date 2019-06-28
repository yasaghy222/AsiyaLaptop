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
    public class MenuController : BaseApiController
    {
        public MenuController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        #region variables
        Tbl_Menu Menu;
        bool HasChild = false;
        #endregion

        #region menu
        [HttpGet]
        public Common.Result Get()
        {
            Data = _unitOfWork.Menu.Get(orderby: item => item.OrderBy(x => x.Sort));
            if (Data != null)
            {
                Result.Message = Common.ResultMessage.OK;
                Result.Data = Data.Adapt<List<Menu.ViewTbl_Menu>>();
            }
            else
            {
                Result.Message = Common.ResultMessage.InternallServerError;
            }
            return Result;
        }

        [HttpPost]
        public async Task<Common.Result> AddEdit([FromBody] Menu.ViewTbl_Menu model)
        {
            if (ModelState.IsValid)
            {
                model.PID = model.PID == 0 ? null : model.PID;
                Menu = await _unitOfWork.Menu.SingleByIdAsync(model.ID);
                if (Menu == null)
                {
                    #region add
                    Menu = model.Adapt<Tbl_Menu>();
                    _unitOfWork.Menu.Add(Menu);
                    #endregion
                }
                else
                {
                    #region edit
                    HasChild = await Task.Run(() => GenericFunction<Tbl_Menu>
                                                    .HasChild(Menu, item => item.PID == Menu.ID));
                    if (!HasChild)
                    {
                        Menu.PID = model.PID;
                    }
                    Menu.Title = model.Title;
                    Menu.Link = model.Link;
                    Menu.Sort = model.Sort;
                    Menu.Status = model.Status;
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
                Menu = await _unitOfWork.Menu.SingleByIdAsync(id);
                if (Menu != null)
                {
                    HasChild = HasChild = await Task.Run(() => GenericFunction<Tbl_Menu>
                                                   .HasChild(Menu, item => item.PID == Menu.ID));
                    if (!HasChild)
                    {
                        _unitOfWork.Menu.Remove(Menu);
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
                        Result.Message = Common.ResultMessage.ChildAssignError;
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