using Src.Models.Service.Repository;
using Src.Models.Utitlity;
using Src.Models.ViewData.Base;
using Src.Models.ViewData.Table;
using System.Threading.Tasks;
using System.Web.Mvc;
using static Src.App_Start.FilterConfig;

namespace Src.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        [HttpGet, PublicAction]
        public ActionResult Index() => View();

        [HttpPost, PublicAction]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(Newsletter.ViewTbl_Newsletter newsletter)
        {
            using var newsletterController = new NewsletterController(_unitOfWork);
            ViewBag.Result = await newsletterController.Register(newsletter);
            return View();
        }

        #region search product
        [HttpPost, PublicAction]
        public async Task<JsonResult> Search(string title)
        {
            if (title == null) return new JsonResult {Data = new Common.Result {Message = Common.ResultMessage.BadRequest}};
            var searchResults = await _unitOfWork.Product.Search(title);
            return new JsonResult
            {
                Data = new Common.Result
                {
                    Message = searchResults == null ? Common.ResultMessage.NotFound : Common.ResultMessage.OK,
                    Data = searchResults
                }
            };
        }

        [HttpGet, PublicAction]
        public async Task<ActionResult> Search(string title = "",
                                               string category = "",
                                               string brand = "",
                                               string filter = "",
                                               long minprice = 0,
                                               long maxprice = 0,
                                               byte pageno = 1,
                                               byte sortby = 0)
        {
            #region set parameters
            var searchParam = new Product.SearchParam(title, category, brand, filter, minprice, maxprice, pageno, sortby);
            var catList = category != "" ? category.Split('-') ?? null : null;
            var cat = catList == null ? category : catList[catList.Length - 1];
            #endregion

            #region search result
            var searchResult = await _unitOfWork.Product.Search(searchParam);
            #endregion

            var searchPageModel = new Product.SearchPageModel
            {
                Params = searchParam,
                CatList = await Function.GetSearchCats(searchParam),
                PropList = await _unitOfWork.Product.GetCatProps(cat),
                BrandList = await _unitOfWork.Product.GetBrands(cat),
                Results = searchResult.Item1,
                SeoTitle = searchResult.Item2,
                ResultCount = searchResult.Item3,
                MaxResultPrice = await _unitOfWork.Product.GetMaxPrice(cat)
            };
            return View(searchPageModel);
        }
        #endregion
    }
}