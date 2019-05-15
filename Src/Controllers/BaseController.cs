using Src.Models.Service.Repository;
using Src.Models.ViewData.Base;
using System.Collections.Specialized;
using System.Web.Mvc;

namespace Src.Controllers
{
    public class BaseController : Controller
    {
        #region variable
        protected object Data;
        protected IUnitOfWork _unitOfWork;
        protected NameValueCollection FormData;
        protected Common.Resualt Resualt = new Common.Resualt();
        #endregion

        public BaseController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
    }
}