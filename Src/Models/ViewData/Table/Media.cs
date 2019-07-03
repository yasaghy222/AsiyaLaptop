using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Src.Models.ViewData.Table
{
    public class Media
    {
        public class ViewTbl_Media
        {
            public int ID { get; set; }
            public string Title { get; set; }
            public string Link { get; set; }
            public byte Sort { get; set; }
            public byte Location { get; set; }
            public string DispLoc { get; set; }
        }

        public enum MediaLocation
        {
            اسلایدر, بنر
        }
    }
}