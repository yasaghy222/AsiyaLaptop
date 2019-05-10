using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Src.Models.ViewData.Table
{
    public class Factor
    {
        public class ViewOrder
        {
            public int ID { get; set; }
            public string Date { get; set; }
            public string CustName { get; set; }
            public string OrderTitles { get; set; }
            public string Status { get; set; }
        }

        public class ViewFullOrder : ViewOrder
        {
            public string PaymentStatus { get; set; }
            public bool IsPrint { get; set; }
            public string Price { get; set; }
        }
    }
}