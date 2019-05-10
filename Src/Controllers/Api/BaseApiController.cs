using Mapster;
using Src.App_Start;
using Src.Models.Service.Repository;
using Src.Models.Utitlity;
using Src.Models.ViewData.Base;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;

namespace Src.Controllers.Api
{
    [Account]
    public class BaseApiController : ApiController
    {
        #region variable
        protected object Data { get; set; }
        protected IUnitOfWork _unitOfWork { get; set; }
        protected NameValueCollection FormData { get; set; }
        protected Common.Resualt Resualt => new Common.Resualt();
        #endregion

        public BaseApiController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        #region common function
        protected ICollection<Common.Select> GetBrandList() => _unitOfWork.ProcBrand.Get().Adapt<ICollection<Common.Select>>();

        protected ICollection<Common.Tree> GetCatList() => _unitOfWork.ProcCat.Get().Adapt<ICollection<Common.Tree>>();
        #endregion
    }
}
