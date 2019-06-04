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
    public class ProductController : BaseController
    {
        #region variable
        Product.ViewTbl_Proc Product = null;
        Product.ViewTbl_ProcCat ProcCat = null;
        Product.ViewTbl_ProcBrand ProcBrand = null;
        #endregion

        [HttpGet]
        public ActionResult Index() => View();

        [HttpGet]
        public ActionResult AddEdit(int? id = -1)
        {
            HttpResponse = Client.GetAsync($"Product/Detail/{Function.GetAdminInfo(Request).Token}?id={id}").Result;
            if (HttpResponse.IsSuccessStatusCode)
            {
                Result = GetResult();
                Product = Result.Data.DeserializeJson<Product.ViewTbl_Proc>();
            }
            else
            {
                ViewBag.Message = Common.ResultMessage.InternallServerError;
                return Redirect("/Admin/Product");
            }

            return View(model: Product);
        }

        [HttpGet]
        public ActionResult Category() => View();

        [HttpGet]
        public ActionResult AddEditCat(int? id = -1)
        {
            HttpResponse = Client.GetAsync($"Product/CatDetail/{Function.GetAdminInfo(Request).Token}?id={id}").Result;
            if (HttpResponse.IsSuccessStatusCode)
            {
                Result = GetResult();
                ProcCat = Result.Data.DeserializeJson<Product.ViewTbl_ProcCat>();
            }
            else
            {
                ViewBag.Message = Common.ResultMessage.InternallServerError;
                return Redirect("/Admin/Product/Category");
            }

            return View(model: ProcCat);
        }

        [HttpGet]
        public ActionResult Brand() => View();
    }
}