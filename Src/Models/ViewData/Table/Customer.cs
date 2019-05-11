using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Src.Models.ViewData.Table
{
    public class Customer
    {
        #region address
        public class ViewOrderAddress
        {
            public string Name { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
        }
        #endregion
    }
}