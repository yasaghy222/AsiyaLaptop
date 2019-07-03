using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Mapster;
using Src.Models.Data;
using Src.Models.Service.Repository;
using Src.Models.Utitlity;
using Src.Models.ViewData.Base;
using Src.Models.ViewData.Table;

namespace Src.Controllers.Api
{
    public class MediaController : BaseApiController
    {
        public MediaController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        #region variables
        Tbl_Media Media;
        #endregion

        #region media
        [HttpGet]
        public Common.Result Get()
        {
            Data = _unitOfWork.Media.Get().OrderBy(x => x.Location).ThenBy(item => item.Sort);
            if (Data != null)
            {
                Result.Message = Common.ResultMessage.OK;
                Result.Data = Data.Adapt<List<Media.ViewTbl_Media>>();
            }
            else
            {
                Result.Message = Common.ResultMessage.InternallServerError;
            }
            return Result;

        }

        [HttpPost]
        public Common.Result AddEdit()
        {
            if (ModelState.IsValid)
            {
                FormData = HttpContext.Current.Request.Form;
                Media = new Tbl_Media
                {
                    ID = int.Parse(FormData.Get("ID")),
                    Title = FormData.Get("Title"),
                    Link = FormData.Get("Link"),
                    Location = byte.Parse(FormData.Get("Location")),
                    Sort = byte.Parse(FormData.Get("Sort"))
                };

                if (Media.ID == 0)
                {
                    #region add
                    _unitOfWork.Media.Add(Media);
                    try
                    {
                        _unitOfWork.Save();
                        Result.Message = Function.UploadImg($"Media/{Media.ID}");
                    }
                    catch (Exception)
                    {
                        Result.Message = Common.ResultMessage.InternallServerError;
                    }
                    #endregion
                }
                else
                {
                    #region edit
                    _unitOfWork.Media.Update(Media);
                    try
                    {
                        _unitOfWork.Save();
                        Result.Message = Function.UpdateImg($"{Media.ID}", "Media");
                    }
                    catch (Exception)
                    {
                        Result.Message = Common.ResultMessage.InternallServerError;
                    }
                    #endregion
                }
            }
            else
            {
                Result.Message = Common.ResultMessage.BadRequest;
            }

            return Result;
        }

        [HttpPost]
        public Common.Result Delete([FromBody]int id)
        {
            if (ModelState.IsValid)
            {
                Media = _unitOfWork.Media.SingleById(id);
                _unitOfWork.Media.Remove(Media);
                try
                {
                    _unitOfWork.Save();
                    Function.DelImg(Media.ID, "Media");
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
        #endregion
    }
}