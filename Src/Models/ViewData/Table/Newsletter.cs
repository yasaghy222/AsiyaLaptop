using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Src.Models.ViewData.Table
{
    public class Newsletter
    {
        public class ViewTbl_Newsletter
        {
            [Required(ErrorMessage ="لطفا ایمیل خود را وارد نمایید.")]
            [EmailAddress(ErrorMessage ="ایمیل وارد شده صحیح نمی باشد.")]
            public string Email { get; set; }
        }
    }
}