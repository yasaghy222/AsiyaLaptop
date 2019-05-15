using Mapster;
using Src.App_Start;
using Src.Models.Service.Repository;
using Src.Models.Utitlity;
using Src.Models.ViewData.Base;
using Src.Models.ViewData.Table;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;

namespace Src.Controllers.Api
{
    [Account]
    public class BaseApiController : ApiController
    {
        #region variable
        protected object Data;
        protected IUnitOfWork _unitOfWork;
        protected NameValueCollection FormData;
        protected Common.Resualt Resualt = new Common.Resualt();
        #endregion

        public BaseApiController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        #region common function
        /// <summary>
        /// return list of product brands
        /// </summary>
        /// <returns></returns>
        protected async Task<ICollection<Common.Select>> GetBrandList() => await Task.Run(() => _unitOfWork.ProcBrand.Get().Adapt<ICollection<Common.Select>>());

        /// <summary>
        /// return list of product cat
        /// </summary>
        /// <returns></returns>
        protected async Task<ICollection<Common.Tree>> GetCatList() => await Task.Run(() => _unitOfWork.ProcCat.Get().Adapt<ICollection<Common.Tree>>());
        #endregion 
    }
}
