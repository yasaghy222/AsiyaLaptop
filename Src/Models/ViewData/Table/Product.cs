using Src.Models.Data;
using Src.Models.ViewData.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Src.Models.ViewData.Table
{
    public class Product
    {
        #region product
        public enum ProcType : byte
        {
            /// <summary>
            /// عادی
            /// </summary>
            [Display(Name = "عادی")]
            normal = 0,

            /// <summary>
            /// پیشنهاد ویژه
            /// </summary>
            [Display(Name = "پیشنهاد ویژه")]
            specialOffer = 1,

            /// <summary>
            /// اتمام موجودی
            /// </summary>
            [Display(Name = "اتمام موجودی")]
            outInventory = 2,

            /// <summary>
            /// توقف تولید
            /// </summary>
            [Display(Name = "توقف تولید")]
            StopProduction = 3
        }

        public class ViewTbl_Proc
        {
            public int ID { get; set; }
            [Required(ErrorMessage = "این فیلد اجباری است.")]
            public string Title { get; set; }
            public string ShortDesc { get; set; }
            public string FullDesc { get; set; }
            [Required(ErrorMessage = "این فیلد اجباری است.")]
            public string TopProp { get; set; }
            public byte Rate { get; set; }
            [Required(ErrorMessage = "این فیلد اجباری است.")]
            public long Price { get; set; }
            public long OffPrice { get; set; }
            public int OffID { get; set; }
            public int BrandID { get; set; }
            public int CatID { get; set; }
            [Required(ErrorMessage = "این فیلد اجباری است.")]
            public int Count { get; set; }
            public int VisitCount { get; set; }
            public byte Type { get; set; }
            public ICollection<Common.Tree> CatList { get; set; }
            public ICollection<Common.Select> BrandList { get; set; }
        }

        public class ViewProc
        {
            public int ID { get; set; }
            public string Title { get; set; }
            public byte Rate { get; set; }
            public string Price { get; set; }
            public string OffPrice { get; set; }
            public string CatName { get; set; }
            public string BrandName { get; set; }
            public int VisitCount { get; set; }
            public string Type { get; set; }
        }
        #endregion

        #region procCat
        public class ViewTbl_ProcCat
        {
            public int ID { get; set; }
            [Required(ErrorMessage = "این فیلد اجباری است.")]
            public string Title { get; set; }
            [Required(ErrorMessage = "این فیلد اجباری است.")]
            public string EnTitle { get; set; }
            public int? PID { get; set; }
            public int AssignCount { get; set; }
            public ICollection<Common.Tree> CatList { get; set; }
        }
        #endregion

        #region procBrand
        public class ViewTbl_ProcBrand
        {
            public int ID { get; set; }
            public string Title { get; set; }
            public string EnTitle { get; set; }
            public int AssignCount { get; set; }
        }
        #endregion

        #region procProp
        public class ViewPropVar
        {
            public int PCPGID { get; set; }
            public int ProcID { get; set; }
            public string Value { get; set; }
        }

        public class ViewProcProp
        {
            public int ID { get; set; }
            public int? PID { get; set; }
            public string Title { get; set; }
            public string Value { get; set; }
            public bool HasChild { get; set; }
        }
        #endregion

        #region pcpGroup
        public class ViewTbl_PCPGroup
        {
            public int ID { get; set; }
            public string Title { get; set; }
            public string EnTitle { get; set; }
            public int PID { get; set; }
            public int CatID { get; set; }
        }

        public class ViewPCPGroup
        {
            public int ID { get; set; }
            public int Title { get; set; }
            public string PName { get; set; }
            public string CatName { get; set; }
        }
        #endregion

        #region search
        public class SearchResult
        {
            public int ID { get; set; }
            public string Title { get; set; }
            public string ShortDesc { get; set; }
        }

        public class FullSearchResult : SearchResult
        {
            public byte Rate { get; set; }
            public string Price { get; set; }
            public string OffPrice { get; set; }
            public int OffID { get; set; }
            public string Type { get; set; }
        }

        public class SearchParam
        {
            public SearchParam(SearchParam searchParam)
            {
                Title = searchParam.Title;
                Category = searchParam.Category;
                Brand = searchParam.Brand;
                MinPrice = searchParam.MinPrice;
                MaxPrice = searchParam.MaxPrice;
                Filter = searchParam.Filter;
                PageNo = searchParam.PageNo;
                SortBy = searchParam.SortBy;
            }

            public SearchParam(
            string title,
            string category,
            string brand,
            string filter,
            long minprice,
            long maxprice,
            byte pageno,
            byte sortby)
            {
                Title = title;
                Category = category;
                Brand = brand;
                MinPrice = minprice;
                MaxPrice = maxprice;
                Filter = filter;
                PageNo = pageno;
                SortBy = sortby;
            }

            public string Title { get; set; }
            public string Category { get; set; }
            public string Brand { get; set; }
            public long MinPrice { get; set; }
            public long MaxPrice { get; set; }
            public string Filter { get; set; }
            public byte PageNo { get; set; }
            public byte SortBy { get; set; }
        }

        public class CatProp
        {
            public int ID { get; set; }
            public string Title { get; set; }
            public string EnTitle { get; set; }
            public List<string> ValueList { get; set; }
        }

        public class Cat : Common.Tree
        {
            public string EnTitle { get; set; }
            public List<Cat> ChildList { get; set; }
            public string Link { get; set; }
        }

        public class SearchPageModel
        {
            public SearchParam Params { get; set; }
            public List<Cat> CatList { get; set; }
            public string SeoTitle { get; set; }
            public int ResultCount { get; set; }
            public List<CatProp> PropList { get; set; }
            public List<ViewTbl_ProcBrand> BrandList { get; set; }
            public List<FullSearchResult> Results { get; set; }
            public long MaxResultPrice { get; set; }
        }

        public enum SortType : byte
        {
            /// <summary>
            /// پربازدید ترین
            /// </summary>
            [Display(Name = "پربازدید ترین")]
            mostVisited = 0,

            /// <summary>
            /// محبوب ترین
            /// </summary>
            [Display(Name = "محبوب ترین")]
            mostPopular = 1,

            /// <summary>
            /// جدید ترین
            /// </summary>
            [Display(Name = "جدید ترین")]
            newest = 2,

            /// <summary>
            /// ارزان ترین
            /// </summary>
            [Display(Name = "ارزان ترین")]
            cheapest = 3,

            /// <summary>
            /// گران ترین
            /// </summary>
            [Display(Name = "گران ترین")]
            mostExpensive = 4,
        }
        #endregion
    }
}