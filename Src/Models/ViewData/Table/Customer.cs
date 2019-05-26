using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public class ViewAccountVar
        {
            [Display(Name = "شماره موبایل")]
            [Required(ErrorMessage = "لطفا شماره موبایل خود را وارد نمایید")]
            [RegularExpression(pattern: @"^09[0-9]{9}$", ErrorMessage = "شماره موبایل وارد شده صحیح نمی باشد")]
            public string Phone { get; set; }
            [Display(Name = "رمز عبور")]
            [Required(ErrorMessage = "لطفا رمز عبور خود را وارد نمایید")]
            public string Pass { get; set; }
            [Display(Name = "نام")]
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