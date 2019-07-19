using Src.Models.Data;
using Src.Models.ViewData.Base;
using Src.Models.ViewData.Table;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Mapster;
using System.Linq.Expressions;
using Src.Models.Utitlity;
using Extensions = Src.Models.Utitlity.Extensions;
using System.Threading.Tasks;

namespace Src.Models.Service.Repository
{
    public class ProductRepository : GenericRepository<Tbl_Product>, IProductRepository
    {
        public ProductRepository(ALDBEntities context) : base(context) => OrderBy = item => item.Title;

        public List<Product.ViewProc> ProcList(Common.TableVar tableVar)
        {
            string query = "";

            if (tableVar != null)
            {
                #region filler
                if (tableVar.Includes != null)
                {
                    int i = 0;
                    query = "where ";
                    foreach (string include in tableVar.Includes.Split(','))
                    {
                        if (i > 0)
                        {
                            query += "or ";
                        }
                        query += $"Title like '%{include}%' ";
                        i++;
                    }
                }
                #endregion
                #region order
                query += $"order by {tableVar.OrderBy} {tableVar.OrderType}";
                #endregion
            }
            else
            {
                tableVar = new Common.TableVar();
            }

            var Data = Context.Tbl_Product.SqlQuery($"Select * from Tbl_Product {query}").ToList();

            try
            {
                #region make pegging
                Data = Data.Skip((tableVar.PageIndex - 1) * tableVar.PageSize)
                           .Take(tableVar.PageSize)
                           .ToList();
                #endregion

                var resualt = Data.Adapt<List<Product.ViewProc>>();
                return resualt;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #region search
        public async Task<List<Product.SearchResult>> Search(string title)
        {
            using (ALDBEntities aLDB = new ALDBEntities())
            {
                List<Product.SearchResult> searchResults =
                    await aLDB.Database
                              .SqlQuery<Product.SearchResult>($"select top(10) ID,Title,ShortDesc from Tbl_Product where Title like '%{title}%'")
                              .ToListAsync();
                return searchResults;
            }
        }

        public async Task<Tuple<List<Product.FullSearchResult>, string, int>> Search(Product.SearchParam searchParam)
        {
            string query = "select * from Tbl_Product ",
                   seoTitle = "فروشگاه لپتاپ آسیا";

            #region title
            if (searchParam.Title != "")
            {
                query += $"where Title like '%{searchParam.Title}%'";
            }
            #endregion

            #region cat 
            if (searchParam.Category != "")
            {
                int catID = 0;
                string[] catList = searchParam.Category.Split('-') ?? null;
                string cat = catList != null ? catList[catList.Length - 1] : searchParam.Category;
                Tbl_ProcCat procCat = await Context.Tbl_ProcCat.SingleOrDefaultAsync(item => item.EnTitle == cat || item.Title == cat);
                if (procCat != null)
                {
                    query += query.Contains("where") ? " and " : "where ";
                    catID = procCat.ID;
                    seoTitle = procCat.SeoDesc;
                    query += $"CatID = {procCat.ID}";
                }
            }
            #endregion

            #region brand
            if (searchParam.Brand != "")
            {
                query += query.Contains("where") ? " and " : "where ";
                string[] brandList = searchParam.Brand.Split(',') ?? null;
                if (brandList != null)
                {
                    query += $"BrandID in (";
                    foreach (string item in brandList)
                    {
                        int? brandID = await Task.Run(() =>
                                            Context.Tbl_ProcBrand.SingleOrDefault(x => x.Title == item || x.EnTitle == item)?.ID);
                        query += $"'{brandID ?? 0}',";
                    }
                    query = query.Substring(0, query.Length - 1) + ")";
                }
                else
                {
                    int? brandID = await Task.Run(() =>
                                            Context.Tbl_ProcBrand.SingleOrDefault(x => x.Title == searchParam.Brand || x.EnTitle == searchParam.Brand)?.ID);
                    query += $"BrandID = {brandID}";
                }
            }
            #endregion

            #region price
            if (searchParam.MinPrice != 0 && searchParam.MaxPrice != 0)
            {
                query += query.Contains("where") ? " and " : "where ";
                query += $"Price between {searchParam.MinPrice} and {searchParam.MaxPrice}";
                query += $"or OffPrice between {searchParam.MinPrice} and {searchParam.MaxPrice}";
            }
            else if (searchParam.MinPrice != 0 && searchParam.MaxPrice == 0)
            {
                query += query.Contains("where") ? " and " : "where ";
                query += $"Price > {searchParam.MinPrice}";
                query += $"or OffPrice > {searchParam.MinPrice}";
            }
            else if (searchParam.MinPrice == 0 && searchParam.MaxPrice != 0)
            {
                query += query.Contains("where") ? " and " : "where ";
                query += $"Price < {searchParam.MaxPrice}";
                query += $"or OffPrice < {searchParam.MaxPrice}";
            }
            #endregion

            #region order
            string sort = "VisitCount";
            switch (searchParam.SortBy)
            {
                case 0:
                    sort = "VisitCount desc";
                    break;
                case 1:
                    sort = "Rate desc";
                    break;
                case 2:
                    sort = "RegDate desc";
                    break;
                case 3:
                    sort = "Price";
                    break;
                case 4:
                    sort = "Price desc";
                    break;
                default:
                    sort = "Title";
                    break;
            }
            query += $" order by {sort}";
            #endregion

            List<Tbl_Product> Data = await Context.Tbl_Product.SqlQuery(query).ToListAsync();

            #region filter
            if (searchParam.Filter != "")
            {
                string[] filterList = searchParam.Filter.Split(',');
                if (filterList != null)
                {
                    foreach (string item in filterList)
                    {
                        if (item != "")
                        {
                            string[] filter = item.Split(':'),
                                     values = filter[1].Split('.');
                            string title = filter[0] ?? "";
                            Data = Data.Where(proc => proc.Tbl_ProcProp.Any(x => x.Tbl_PCPGroup.EnTitle == title && values.Contains(x.Value))).ToList();
                        }
                    }
                }
                else
                {
                    string[] filter = searchParam.Filter.Split(':'),
                                values = filter[1].Split('.');
                    string title = filter[0] ?? "";
                    Data = Data.Where(proc => proc.Tbl_ProcProp.Any(x => x.Tbl_PCPGroup.EnTitle == title && values.Contains(x.Value))).ToList();
                }
            }
            #endregion

            #region make paging
            int count = Data.Count;
            Data = Data.Skip((searchParam.PageNo - 1) * 10)
                       .Take(10)
                       .ToList();
            #endregion

            List<Product.FullSearchResult> searchResults = Data.Adapt<List<Product.FullSearchResult>>();
            return new Tuple<List<Product.FullSearchResult>, string, int>(searchResults, seoTitle, count);
        }

        public async Task<long> GetMaxPrice(string catName)
        {
            List<Tbl_Product> products = await Context.Tbl_Product.Where(item => item.Tbl_ProcCat.Title == catName).ToListAsync();
            return products.Count() > 0 ? products.Max(item => item.Price) : await Context.Tbl_Product.MaxAsync(item => item.Price);
        }

        public async Task<List<Product.CatProp>> GetCatProps(string catName)
        {
            var cat = await Context.Tbl_ProcCat.SingleOrDefaultAsync(item => item.EnTitle == catName || item.Title == catName);
            if (cat != null)
            {
                var result = new List<Product.CatProp>();
                var catProps = await Context.Tbl_PCPGroup.Where(item => item.CatID == cat.ID).ToListAsync();
                var selfProps = catProps.Count() > 0 ? catProps.Adapt<List<Product.CatProp>>() : null;
                if (selfProps !=null)
                {
                    result.AddRange(selfProps);
                }

                if (cat.PID != null)
                {
                    string parentName = cat.Tbl_ProcCat2.EnTitle;
                    var parentProps = await GetCatProps(parentName);
                    if (parentProps != null)
                    {
                        result.AddRange(parentProps);
                    }
                }
                return result;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<Product.ViewTbl_ProcBrand>> GetBrands(string catName)
        {
            string query = $"SELECT DISTINCT dbo.Tbl_ProcBrand.ID, dbo.Tbl_ProcBrand.Title, dbo.Tbl_ProcBrand.EnTitle, dbo.Tbl_ProcCat.EnTitle FROM dbo.Tbl_Product INNER JOIN dbo.Tbl_ProcBrand ON dbo.Tbl_Product.BrandID = dbo.Tbl_ProcBrand.ID INNER JOIN dbo.Tbl_ProcCat ON dbo.Tbl_Product.CatID = dbo.Tbl_ProcCat.ID Where  dbo.Tbl_ProcCat.EnTitle ='{catName}'";
            List<Product.ViewTbl_ProcBrand> brands = await Context.Database.SqlQuery<Product.ViewTbl_ProcBrand>(query).ToListAsync();
            if (brands.Count == 0 || brands == null)
            {
                query = $"select dbo.Tbl_ProcBrand.ID, dbo.Tbl_ProcBrand.Title, dbo.Tbl_ProcBrand.EnTitle from dbo.Tbl_ProcBrand";
                brands = await Context.Database.SqlQuery<Product.ViewTbl_ProcBrand>(query).ToListAsync();
            }
            return brands;
        }
        #endregion
    }
}