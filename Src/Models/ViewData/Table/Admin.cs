using System.ComponentModel.DataAnnotations;

namespace Src.Models.ViewData.Table
{
    public class Admin
    {
        public class ViewTbl_Admin
        {
            public int ID { get; set; }
            [Required(ErrorMessage = "این فیلد اجباری است.")]
            public string Name { get; set; }
            [Required(ErrorMessage = "این فیلد اجباری است.")]
            public string Family { get; set; }
            public string RoleName { get; set; }
            public int RoleID { get; set; }
            [Required(ErrorMessage = "لطفا شماره موبایل خود را وارد نمایید")]
            [RegularExpression(pattern: @"^09[0-9]{9}$", ErrorMessage = "شماره موبایل وارد شده صحیح نمی باشد")]
            public string Phone { get; set; }
            [Required(ErrorMessage = "لطفا کدملی را وارد نمایید")]
            [MinLength(10, ErrorMessage = "کدملی وارد شده صحیح نمی باشد.")]
            [MaxLength(10, ErrorMessage = "کدملی وارد شده صحیح نمی باشد.")]
            public string NatCode { get; set; }
            [Required(ErrorMessage = "این فیلد اجباری است.")]
            public string BDate { get; set; }
            public string Pass { get; set; }
            public bool Status { get; set; }
            public string Token { get; set; }
        }

        public class ViewAdmin
        {
            public string Token { get; set; }
            public string FullName { get; set; }
            public string RoleName { get; set; }
            public int RoleID { get; set; }
            public bool Status { get; set; }
        }

        public class LoginVar
        {
            [Required(ErrorMessage = "لطفا شماره موبایل خود را وارد نمایید")]
            [RegularExpression(pattern: @"^09[0-9]{9}$", ErrorMessage = "شماره موبایل وارد شده صحیح نمی باشد")]
            public string Phone { get; set; }
            [Required(ErrorMessage = "لطفا رمز عبور خود را وارد نمایید")]
            public string Pass { get; set; }
        }
    }
}