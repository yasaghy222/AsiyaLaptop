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
    }
}