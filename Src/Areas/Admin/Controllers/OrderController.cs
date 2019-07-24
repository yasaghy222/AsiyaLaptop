using System;
using System.Collections.Generic;
using System.Linq;
using Src.Models.Utitlity;
using Src.Models.ViewData.Base;
using Src.Models.ViewData.Table;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.IO;
using Mapster;
using Newtonsoft.Json;

namespace Src.Areas.Admin.Controllers
{
    public class OrderController : BaseController
    {
        [HttpGet]
        public ActionResult Index() => View();

        [HttpGet]
        public async Task<ActionResult> Detail(int id = 0)
        {
            if (id == 0) return Redirect("/Admin/Order");

            _HttpResponse = await Client.GetAsync($"Order/Detail/{Function.GetAdminInfo(Request).Token}?id={id}");
            if (_HttpResponse.IsSuccessStatusCode)
            {
                Result = GetResult();
                var detail = Result.Data.DeserializeJson<Factor.ViewOrderDetail>();
                return View(detail);
            }

            ViewBag.Message = Common.ResultMessage.InternallServerError;
            return Redirect("/Admin/Order");
        }
    }
}
