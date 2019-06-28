using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using  Src.Models.ViewData.Base;

namespace Src.Models.ViewData.Table
{
    public class Menu
    {
        public class ViewTbl_Menu
        {
            public int ID { get; set; }
            public int? PID { get; set; }
            [Required(ErrorMessage = "این فیلد اجباری است.")]
            public string Title { get; set; }
            public string Link { get; set; }
            public int? Sort { get; set; }
            public bool Status { get; set; }
            public bool HasChild { get; set; }
        }
    }
}