using Mapster;
using Src.Models.Data;
using Src.Models.Service.Repository;
using Src.Models.Utitlity;
using Src.Models.ViewData.Base;
using Src.Models.ViewData.Table;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Src.Controllers
{
    public class ProductController : BaseController
    {
        public ProductController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        #region search
        [HttpPost, PublicAction, ValidateModel]
        public async Task<JsonResult> Search(string title)
        {
            if (title != null)
            {
                List<Product.SearchResult> searchResults = await _unitOfWork.Product.Search(title);
                return new JsonResult
                {
                    Data = new Common.Result
                    {
                        Message = searchResults != null ? Common.ResultMessage.OK : Common.ResultMessage.NotFound,
                        Data = searchResults ?? null
                    }
                };
            }
            else
            {
                return new JsonResult { Data = new Common.Result { Message = Common.ResultMessage.BadRequest } };
            }
        }
        #endregion

        #region search with full details
        [HttpGet, PublicAction, ValidateModel]
        [Route("Search")]
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
            Product.SearchParam searchParam = new Product.SearchParam(title, category, brand, filter, minprice, maxprice, pageno, sortby);
            string[] catList = category != "" ? category.Split('-') ?? null : null;
            _ = brand != "" ? brand.Split(',') ?? null : null;
            string cat = catList == null ? category : catList[catList.Length - 1];
            #endregion

            #region search result
            var searchResult = await _unitOfWork.Product.Search(searchParam);
            #endregion

            Product.SearchPageModel searchPageModel = new Product.SearchPageModel
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

        #region get newest products
        public static List<Product.FullSearchResult> GetNewest()
        {
            using (ALDBEntities aLDB = new ALDBEntities())
            {
                var data = aLDB.Tbl_Product.OrderBy(item => item.RegDate);
                return data.Take(4).Adapt<List<Product.FullSearchResult>>();
            }
        }
        #endregion

        #region get best selling products
        public static List<Product.FullSearchResult> GetBestSelling()
        {
            using (ALDBEntities aLDB = new ALDBEntities())
            {
                var data = aLDB.Tbl_Product.Where(item => item.Tbl_FactProc.Count > 0)
                                           .OrderByDescending(item => item.Tbl_FactProc.Count);
                return data.Take(4).Adapt<List<Product.FullSearchResult>>();
            }
        }
        #endregion

        #region get most visited products
        public static List<Product.FullSearchResult> GetMostVisited()
        {
            using (ALDBEntities aLDB = new ALDBEntities())
            {
                var data = aLDB.Tbl_Product.OrderByDescending(item => item.VisitCount);
                return data.Take(4).Adapt<List<Product.FullSearchResult>>();
            }
        }
        #endregion

        #region get most popular products
        public static List<Product.FullSearchResult> GetMostPopular()
        {
            using (ALDBEntities aLDB = new ALDBEntities())
            {
                var data = aLDB.Tbl_Product.OrderByDescending(item => item.Rate);
                return data.Take(4).Adapt<List<Product.FullSearchResult>>();
            }
        }
        #endregion

        #region get popular brands
        public static List<Product.Brand> GetPopularBrand()
        {
            using (ALDBEntities aLDB = new ALDBEntities())
            {
                return aLDB.Database
                           .SqlQuery<Product.Brand>(@"
                                                      select Top 4 
                                                      ID,Title,EnTitle 
                                                      from Tbl_ProcBrand 
                                                      join (
                                                            select max(Rate) Rate,BrandID 
                                                            from Tbl_Product
                                                            group by BrandID
                                                      ) product
                                                      on Tbl_ProcBrand.ID = product.BrandID")
                           .ToList();
            }
        }
        #endregion
    }
}