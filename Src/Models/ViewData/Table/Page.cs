using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Src.Models.ViewData.Table
{
    public class Page
    {
        public class ViewPage
        {
            [Required(ErrorMessage = "این فیلد اجباری است.")]
            public string Title { get; set; }
            [Required(ErrorMessage = "این فیلد اجباری است.")]
            public string Link { get; set; }
            public string SeoDesc { get; set; }
            public string Keywords { get; set; }
            public string Body { get; set; }
        }

        public class ViewTbl_Page : ViewPage
        {
            public int ID { get; set; }
            public bool Status { get; set; }
        }
    }
}