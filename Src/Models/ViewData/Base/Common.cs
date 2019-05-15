using System.Collections.Generic;
using System.Net;

namespace Src.Models.ViewData.Base
{
    public class Common
    {
        public class Resualt
        {
            public Resualt() => Data = null;
            public string Message { get; set; }
            public object Data { get; set; }
        }

        public abstract class ResualtMessage
        {
            public const string OK = "Success";
            public const string NotFound = "داده ای یافت نشد.";
            public const string BadRequest = "ورودی ها صحیح نمی باشد.";
            public const string ChildAssignError = "رکورد دارای فرزند و یا اختصاص قابل حذف نیست!";
            public const string InternallServerError = "مشکلی پیش آمده است لطفا در زمان دیگری اقدام نمایید.";
        }

        public static List<string> ImgValidType { get; } = new List<string>
        {
            "image/png",
            "image/jpg",
            "image/jpeg",
            "image/gif"
        };

        public class TableVar
        {
            public TableVar()
            {
                PageIndex = 1;
                PageSize = 10;
                OrderBy = "first";
                OrderType = "desc";
                Includes = null;
            }
            public int PageIndex { get; set; }
            public int PageSize { get; set; }
            public string OrderBy { get; set; }
            public string OrderType { get; set; }
            public string Includes { get; set; }
        }

        public class Select
        {
            public int ID { get; set; }
            public string Title { get; set; }
        }

        public class SelectWithProc : Select
        {
            public int ProcID { get; set; }
        }

        public class Tree
        {
            public int ID { get; set; }
            public int? PID { get; set; }
            public string Title { get; set; }
        }

        public class FullTree : Tree
        {
            public int AssignCount { get; set; }

            public bool HasChild { get; set; }
        }
    }
}