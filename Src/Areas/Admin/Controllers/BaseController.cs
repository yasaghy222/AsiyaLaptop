using Src.Models.Service.Repository;
using Src.Models.Utitlity;
using Src.Models.ViewData.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Src.Models.ViewData;
using Newtonsoft.Json;

namespace Src.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        #region variable
        protected Common.Resualt Resualt = null;
        protected HttpClient Client = new HttpClient();
        protected HttpResponseMessage HttpResponse = null;
        #endregion

        #region general functions
        protected Common.Resualt GetResualt()
        {
            Task<string> data = HttpResponse.Content.ReadAsStringAsync();
            Resualt = JsonConvert.DeserializeObject<Common.Resualt>(data.Result.ToString());
            return Resualt;
        }
        #endregion

        protected BaseController()
        {
            #region api configure
            Client.BaseAddress = new Uri("http://localhost:4219/Api/V1/");
            Client.DefaultRequestHeaders.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            #endregion
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            #region get admin info
            Models.ViewData.Table.Admin.AInfo AInfo = Function.GetAdminInfo(Request);
            ViewBag.FullName =  AInfo.FullName;
            ViewBag.RoleName = AInfo.RoleName;
            ViewBag.RoleID = AInfo.RoleID;
            ViewBag.Token = AInfo.Token;
            #endregion

            base.OnActionExecuted(filterContext);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Client != null)
                {
                    Client.Dispose();
                    Client = null;
                }
            }
        }
    }
}