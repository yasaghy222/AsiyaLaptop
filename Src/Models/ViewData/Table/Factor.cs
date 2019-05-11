using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Src.Models.ViewData.Table
{
    public class Factor
    {
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
            public string Price { get; set; }
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