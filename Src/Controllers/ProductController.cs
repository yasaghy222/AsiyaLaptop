using Mapster;
using Src.Models.Data;
using Src.Models.Service.Repository;
using Src.Models.Utitlity;
using Src.Models.ViewData.Table;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using static Src.App_Start.FilterConfig;
using static Src.Models.ViewData.Table.Product;

namespace Src.Controllers
{
    public class ProductController : BaseController
    {
        public ProductController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        [HttpGet, PublicAction]
        public async Task<ActionResult> Index(string id = "")
        {
            string[] index = { "index", "Index" };
            if (id != "" && !index.Contains(id))
            {
                int procID = int.Parse(id.Split('-')[1]);
                var Data = await _unitOfWork.Product.SingleByIdAsync(procID);
                ProcPageModel procPageModel = new ProcPageModel
                {
                    Proc = Data.Adapt<ViewFullProc>(),
                    Cats = await Data.Tbl_ProcCat.GetProcPageCats(),
                    Imgs = await Task.Run(() => _unitOfWork.ProcImg.Get(item => item.ProcID == Data.ID).ToList()),
                    Props = await _unitOfWork.PCPGroup.GetPCPGs(Data),
                    RelatedProcs = await Task.Run(() => _unitOfWork.Product.Get(item => item.ID != Data.ID && item.CatID == Data.CatID).Adapt<List<FullSearchResult>>()),
                    Reviews = await Task.Run(() => _unitOfWork.ProcReview.Get(item => item.ProcID == Data.ID).Adapt<List<ViewProcReview>>())
                };

                await AddVisitCount(Data);
                ViewBag.Title = Data.Title;
                return View(procPageModel);
            }
            else
            {
                return Redirect("/");
            }
        }

        #region add visit count
        private async Task AddVisitCount(Tbl_Product product)
        {
            if (product != null)
            {
                product.VisitCount++;
                try
                {
                    await _unitOfWork.SaveAsync();
                }
                catch (System.Exception)
                {
                    throw;
                }
            }
        }
        #endregion

        #region get newest products
        public static List<FullSearchResult> GetNewest()
        {
            using (ALDBEntities aLDB = new ALDBEntities())
            {
                var data = aLDB.Tbl_Product.OrderBy(item => item.RegDate);
                return data.Take(4).Adapt<List<FullSearchResult>>();
            }
        }
        #endregion

        #region get best selling products
        public static List<FullSearchResult> GetBestSelling()
        {
            using var aLDB = new ALDBEntities();
            var data = aLDB.Tbl_FactProc.Where(item => item.Tbl_Factor.Status == (byte)Factor.FactStatus.DeliveryToCust)
                .Select(item => item.Tbl_Product)
                .OrderByDescending(item => item.ExistCount);
            return data.Take(4).Adapt<List<FullSearchResult>>();
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
        public static List<Brand> GetPopularBrand()
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