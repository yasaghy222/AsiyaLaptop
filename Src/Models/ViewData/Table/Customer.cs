using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Src.Models.ViewData.Table
{
    public class Customer
    {
        #region customer
        public class CustomerInfo
        {
            public string Token { get; set; }
            public string Name { get; set; }
            public string Family { get; set; }
            public string Phone { get; set; }
        }

        public class ViewLoginVar
        {
            public string Phone { get; set; }
            public string Pass { get; set; }
        }

        public class ViewRegisterVar : ViewLoginVar
        {
            public string Name { get; set; }
        }
        #endregion

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