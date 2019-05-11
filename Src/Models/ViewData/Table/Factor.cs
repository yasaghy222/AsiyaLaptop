using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Src.Models.ViewData.Table
{
    public class Factor
    {
        #region general
        public static class PaymentStatus
        {
            /// <summary>
            /// پرداخت شده
            /// </summary>
            public static string Paid => "پرداخت شده";
            /// <summary>
            /// عدم پرداخت
            /// </summary>
            public static string UnPaid => "عدم پرداخت";
        }

        public enum FactStatus : byte
        {
            /// <summary>
            /// در سبد خرید
            /// </summary>
            [Display(Name = "در سبد خرید")]
            InBascket = 0,

            /// <summary>
            /// تایید سفارش
            /// </summary>
            [Display(Name = "تایید سفارش")]
            Confirm = 1,

            /// <summary>
            /// در حال آماده سازی
            /// </summary>
            [Display(Name = "در حال آماده سازی")]
            InProsses = 2,

            /// <summary>
            /// تحویل به پست
            /// </summary>
            [Display(Name = "تحویل به پست")]
            DeliveryToPost = 3,

            /// <summary>
            /// تحویل به مشتری
            /// </summary>
            [Display(Name = "تحویل به مشتری")]
            DeliveryToCust = 4,

            /// <summary>
            /// مرجوعی
            /// </summary>
            [Display(Name = "مرجوعی")]
            Returned = 5,

            /// <summary>
            /// لغو شده
            /// </summary>
            [Display(Name = "لغو شده")]
            Canceled = 6
        }
        #endregion

        #region order
        public class ViewOrder
        {
            public int ID { get; set; }
            public string SubmitDate { get; set; }
            public string CustName { get; set; }
            public string PaymentStatus { get; set; }
            public string Status { get; set; }
        }

        public class ViewFullOrder : ViewOrder
        {
            public bool IsPrint { get; set; }
            public string TotalPrice { get; set; }
        }

        public class ViewOrderDetail : ViewFullOrder
        {
            public Customer.ViewOrderAddress CustAddress { get; set; }
            public ICollection<OrderProc> OrderProc { get; set; }
        }
        #endregion

        #region factProc
        public class OrderProc
        {
            public int ID { get; set; }
            public string Title { get; set; }
            public string TopProp { get; set; }
            public string Count { get; set; }
            public string ValuePrice { get; set; }
            public string TotalPrice { get; set; }
            public string OffPrice { get; set; }
            public string FinalPrice { get; set; }
        }
        #endregion
    }
}