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
        public class ViewTbl_Customer
        {
            public int ID { get; set; }
            public string IP { get; set; }
            public string Name { get; set; }
            public string Family { get; set; }
            public string Phone { get; set; }
            public string NatCode { get; set; }
            public string Email { get; set; }
            public bool IsInNewsletter { get; set; }
            public bool Status { get; set; }
        }

        public class ViewCustomer
        {
            public int ID { get; set; }
            [Required(ErrorMessage = "لطفا نام خود را وارد نمایید")]
            public string Name { get; set; }
            [Required(ErrorMessage = "لطفا نام خانوادگی خود را وارد نمایید")]
            public string Family { get; set; }
            [Display(Name = "شماره موبایل")]
            [Required(ErrorMessage = "لطفا شماره موبایل خود را وارد نمایید")]
            [RegularExpression(pattern: @"^09[0-9]{9}$", ErrorMessage = "شماره موبایل وارد شده صحیح نمی باشد")]
            public string Phone { get; set; }
            [Required(ErrorMessage = "لطفا کد ملی خود را وارد نمایید")]
            [MaxLength(10,ErrorMessage = "کد ملی وارد شده صحیح نمی باشد")]
            [MinLength(10, ErrorMessage = "کد ملی وارد شده صحیح نمی باشد")]
            public string NatCode { get; set; }
            [EmailAddress(ErrorMessage = "ایمیل وارد شده صحیح نمی باشد")]
            public string Email { get; set; }
            public bool IsInNewsletter { get; set; }
        }

        public class CustomerInfo
        {
            public string Token { get; set; }
            public string Name { get; set; }
            public string Family { get; set; }
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