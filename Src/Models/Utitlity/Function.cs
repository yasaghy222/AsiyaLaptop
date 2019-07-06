using Mapster;
using Src.Models.Data;
using Src.Models.ViewData.Base;
using Src.Models.ViewData.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Hosting;
using static Src.Models.Utitlity.GuidFunction;

namespace Src.Models.Utitlity
{
    public class Function
    {
        #region general
        /// <summary>
        /// هش کردن پارامتر ورودی
        /// </summary>
        /// <param name="code">پارامتر ورودی جهت هش کردن</param>
        /// <returns></returns>
        public static string GenerateHash(string code)
        {
            UTF8Encoding ue = new UTF8Encoding();
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            //first hash
            byte[] firsthash = ue.GetBytes(code);
            byte[] firsthashing = md5.ComputeHash(firsthash);

            string first = System.Text.RegularExpressions.Regex.Replace(BitConverter.ToString(firsthashing), "-", "").ToLower();

            //second hash
            byte[] secondhash = ue.GetBytes(first);
            byte[] secondhashing = md5.ComputeHash(secondhash);

            string second = System.Text.RegularExpressions.Regex.Replace(BitConverter.ToString(secondhashing), "-", "").ToLower();

            return second;
        }

        /// <summary>
        /// کاربر ip دریافت آدرس
        /// </summary>
        /// <returns></returns>
        public static string GetIP()
        {
            string VisitorsIPAddr = string.Empty;
            if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {
                VisitorsIPAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else if (HttpContext.Current.Request.UserHostAddress.Length != 0)
            {
                VisitorsIPAddr = HttpContext.Current.Request.UserHostAddress;
            }
            return VisitorsIPAddr;
        }
        #endregion

        #region code
        /// <summary>
        /// تولید کد عددی غیر تکراری
        /// </summary>
        /// <returns></returns>
        public static int GenerateNumCode()
        {
            string temp = DateTime.Now.Millisecond.ToString();
            temp = temp.Substring(0, temp.Length / 2);
            temp = $"{DateTime.Now.ToPersianDate("short")}{temp}";
            return int.Parse(temp);
        }

        /// <summary>
        /// تولید توکن جدید
        /// </summary>
        /// <returns></returns>
        public static string GenerateNewToken()
        {
            ShortGuid guid = Guid.NewGuid();
            return guid;
        }

        /// <summary>
        /// تولید کد عددی برای اعتبار سنجی
        /// </summary>
        /// <returns></returns>
        public static string GenerateValidationCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }
        #endregion

        #region image
        /// <summary>
        /// چک کردن تصویر
        /// </summary>
        /// <param name="img">فایل تصویر</param>
        /// <returns></returns>
        public static string ValidateImg(HttpPostedFile img)
        {
            string temp;
            HttpRequest request = HttpContext.Current.Request;
            if (request.Files.Count > 0)
            {
                if ((img.ContentLength / 1024) <= 4096)
                {
                    if (Common.ImgValidType.Contains(img.ContentType))
                    {
                        temp = "Success";
                    }
                    else
                    {
                        temp = "فرمت تصویر مجاز نمی باشد.";
                    }
                }
                else
                {
                    temp = "اندازه تصویر بیش از حد مجاز است.";
                }
            }
            else
            {
                temp = "تصویر یافت نشد.";
            }
            return temp;
        }

        /// <summary>
        /// جستجوی تصویر
        /// </summary>
        /// <param name="id">شناسه تصویر</param>
        /// <param name="path">آدرس تصویر</param>
        /// <returns></returns>
        public static bool AnyImg(object id, string path) => File.Exists(HostingEnvironment.MapPath($"~/Files/{path}/{id}.jpg"));

        /// <summary>
        /// آپلود کردن تصویر
        /// </summary>
        /// <param name="path">آدرس</param>
        /// <returns></returns>
        public static string UploadImg(string path)
        {
            string temp = "";
            HttpFileCollection Files = HttpContext.Current.Request.Files;
            for (int i = 0; i < Files.Count; i++)
            {
                HttpPostedFile img = Files[i];
                temp = ValidateImg(img);
                if (temp == "Success")
                {
                    img.SaveAs(HostingEnvironment.MapPath($"~/Files/{path}.jpg"));
                }
            }
            return temp;
        }

        /// <summary>
        /// حذف تصاویر از سرور
        /// </summary>
        /// <param name="id">شناسه آغازین تصاویر</param>
        /// <param name="path">آدرس</param>
        public static void DelImg(object id, string[] path)
        {
            foreach (string item in path)
            {
                Directory.Delete(HostingEnvironment.MapPath($"~/Files/{item}/{id}.jpg"));
            }
        }

        /// <summary>
        /// حذف تصویر از سرور
        /// </summary>
        /// <param name="id">شناسه اغازین تصویر</param>
        /// <param name="path">آدرس</param>
        public static void DelImg(object id, string path)
        {
            if (AnyImg(id, path))
            {
                File.Delete(HostingEnvironment.MapPath($"~/Files/{path}/{id}.jpg"));
            }
        }

        /// <summary>
        /// به روزرسانی تصویر
        /// </summary>
        /// <param name="id">شناسه اغازین تصویر</param>
        /// <param name="path">آدرس</param>
        /// <returns>وضعیت</returns>
        public static string UpdateImg(object id, string path)
        {
            string temp = "";
            HttpFileCollection Files = HttpContext.Current.Request.Files;
            if (Files.Count > 0)
            {
                for (int i = 0; i < Files.Count; i++)
                {
                    HttpPostedFile img = Files[i];
                    temp = ValidateImg(img);
                    if (temp == Common.ResultMessage.OK)
                    {
                        DelImg(id, path);
                        img.SaveAs(HostingEnvironment.MapPath($"~/Files/{path}/{id}.jpg"));
                    }
                }
            }
            else
            {
                temp = Common.ResultMessage.OK;
            }
            return temp;
        }
        #endregion

        #region project
        /// <summary>
        /// دریافت اطلاعات کاربر مدیر
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Admin.ViewAdmin GetAdminInfo(HttpRequestBase request)
        {
            Admin.ViewAdmin Item = null;
            if (request.Cookies?.Get("ALAdminInfo") != null)
            {
                Item = new Admin.ViewAdmin
                {
                    FullName = request.Cookies["ALAdminInfo"]["FullName"],
                    RoleName = request.Cookies["ALAdminInfo"]["RoleName"],
                    RoleID = int.Parse(request.Cookies["ALAdminInfo"]["RoleID"]),
                    Token = request.Cookies["ALAdminInfo"]["Token"]
                };
            }
            return Item;
        }

        /// <summary>
        /// دریافت آدرس جستجوی محصولات
        /// </summary>
        /// <param name="category"></param>
        /// <param name="brand"></param>
        /// <param name="minprice"></param>
        /// <param name="maxprice"></param>
        /// <param name="filter"></param>
        /// <param name="pageno"></param>
        /// <param name="sortby"></param>
        /// <returns></returns>
        public static string GetSearchLink(Product.SearchParam searchParam)
        {
            string url = "/Search?";

            #region category
            url += searchParam.Category != "" ? $"category={searchParam.Category}" : "";
            #endregion

            #region brand
            url += searchParam.Brand != "" ? $"&brand={searchParam.Brand}" : "";
            #endregion

            #region price
            url += searchParam.MinPrice != 0 ? $"&minprice={searchParam.MinPrice}" : "";
            url += searchParam.MaxPrice != 0 ? $"&maxprice={searchParam.MaxPrice}" : "";
            #endregion

            #region filter
            url += searchParam.Filter != "" ? $"&filter={searchParam.Filter}" : "";
            #endregion

            #region pageno
            url += searchParam.PageNo != 1 ? $"&pageno={searchParam.PageNo}" : "";
            #endregion

            #region sortby
            url += searchParam.SortBy != 0 ? $"&sortby={searchParam.SortBy}" : "";
            #endregion

            url = url.Length == 8 ? url.Substring(0, 7) : url;
            return url;
        }
        #endregion

        /// <summary>
        /// get single category of products
        /// </summary>
        /// <param name="searchParam"></param>
        /// <param name="catTitle"></param>
        /// <returns></returns>
        public static async Task<Product.Cat> GetSearchCat(Product.SearchParam searchParam, string catTitle)
        {
            using (ALDBEntities aLDB = new ALDBEntities())
            {
                var Cat = await Task.Run(() => aLDB.Tbl_ProcCat.SingleOrDefault(x => x.Title == catTitle || x.EnTitle == catTitle));
                if (Cat != null)
                {
                    var ChildList = await Task.Run(() => aLDB.Tbl_ProcCat.Where(x => x.PID == Cat.ID && x.Tbl_Product.Count > 0).ToList());
                    if (searchParam.Category.Contains('-'))
                    {
                        string temp = searchParam.Category.SubStrFirst(catTitle);
                        searchParam.Category = temp == "" ? catTitle : temp + catTitle;
                    }
                    return new Product.Cat
                    {
                        ID = Cat.ID,
                        PID = Cat.PID,
                        Title = Cat.Title,
                        EnTitle = Cat.EnTitle,
                        ChildList = ChildList.Adapt<List<Product.Cat>>(),
                        Link = GetSearchLink(searchParam)
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// get search product category list
        /// </summary>
        /// <param name="searchParam"></param>
        /// <returns></returns>
        public static async Task<List<Product.Cat>> GetSearchCats(Product.SearchParam searchParam)
        {
            List<Product.Cat> catList = null;
            if (searchParam.Category != "")
            {
                catList = new List<Product.Cat>();
                string[] cats = searchParam.Category.Contains("-") ? searchParam.Category.Split('-') : null;
                if (cats != null)
                {
                    string oldCat = searchParam.Category;
                    foreach (string item in cats)
                    {
                        searchParam.Category = oldCat;
                        Product.Cat cat = await GetSearchCat(searchParam, item);
                        if (cat != null)
                        {
                            catList.Add(cat);
                        }
                    }
                }
                else
                {
                    catList.Add(await GetSearchCat(searchParam, searchParam.Category));
                }
            }
            return catList;
        }
    }

    public static class GenericFunction<T> where T : class
    {
        /// <summary>
        /// بررسی تعداد فرزندان یک انتیتی
        /// </summary>
        /// <param name="item">انتیتی</param>
        /// <param name="exp">شرط</param>
        /// <returns></returns>
        public static bool HasChild(T item, Expression<Func<T, bool>> exp)
        {
            if (item != null)
            {
                using (ALDBEntities db = new ALDBEntities())
                {
                    int count = db.Set<T>().Count(exp);
                    return count == 0 ? false : true;
                }
            }
            return false;
        }
    }

    public class GuidFunction
    {
        /// <summary>
        /// Represents a globally unique identifier (GUID) with a
        /// shorter string value. Sguid
        /// </summary>
        public struct ShortGuid
        {
            #region Static

            /// <summary>
            /// A read-only instance of the ShortGuid class whose value
            /// is guaranteed to be all zeroes.
            /// </summary>
            public static readonly ShortGuid Empty = new ShortGuid(Guid.Empty);

            #endregion

            #region Fields

            Guid _guid;
            string _value;

            #endregion

            #region Contructors

            /// <summary>
            /// Creates a ShortGuid from a base64 encoded string
            /// </summary>
            /// <param name="value">The encoded guid as a
            /// base64 string</param>
            public ShortGuid(string value)
            {
                _value = value;
                _guid = Decode(value);
            }

            /// <summary>
            /// Creates a ShortGuid from a Guid
            /// </summary>
            /// <param name="guid">The Guid to encode</param>
            public ShortGuid(Guid guid)
            {
                _value = Encode(guid);
                _guid = guid;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets/sets the underlying Guid
            /// </summary>
            public Guid Guid
            {
                get { return _guid; }
                set
                {
                    if (value != _guid)
                    {
                        _guid = value;
                        _value = Encode(value);
                    }
                }
            }

            /// <summary>
            /// Gets/sets the underlying base64 encoded string
            /// </summary>
            public string Value
            {
                get { return _value; }
                set
                {
                    if (value != _value)
                    {
                        _value = value;
                        _guid = Decode(value);
                    }
                }
            }

            #endregion

            #region ToString

            /// <summary>
            /// Returns the base64 encoded guid as a string
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return _value;
            }

            #endregion

            #region Equals

            /// <summary>
            /// Returns a value indicating whether this instance and a
            /// specified Object represent the same type and value.
            /// </summary>
            /// <param name="obj">The object to compare</param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                if (obj is ShortGuid)
                    return _guid.Equals(((ShortGuid)obj)._guid);
                if (obj is Guid)
                    return _guid.Equals((Guid)obj);
                if (obj is string)
                    return _guid.Equals(((ShortGuid)obj)._guid);
                return false;
            }

            #endregion

            #region GetHashCode

            /// <summary>
            /// Returns the HashCode for underlying Guid.
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return _guid.GetHashCode();
            }

            #endregion

            #region NewGuid

            /// <summary>
            /// Initialises a new instance of the ShortGuid class
            /// </summary>
            /// <returns></returns>
            public static ShortGuid NewGuid()
            {
                return new ShortGuid(Guid.NewGuid());
            }

            #endregion

            #region Encode

            /// <summary>
            /// Creates a new instance of a Guid using the string value,
            /// then returns the base64 encoded version of the Guid.
            /// </summary>
            /// <param name="value">An actual Guid string (i.e. not a ShortGuid)</param>
            /// <returns></returns>
            public static string Encode(string value)
            {
                Guid guid = new Guid(value);
                return Encode(guid);
            }

            /// <summary>
            /// Encodes the given Guid as a base64 string that is 22
            /// characters long.
            /// </summary>
            /// <param name="guid">The Guid to encode</param>
            /// <returns></returns>
            public static string Encode(Guid guid)
            {
                string encoded = Convert.ToBase64String(guid.ToByteArray());
                encoded = encoded
                    .Replace("/", "_")
                    .Replace("+", "-");
                return encoded.Substring(0, 22);
            }

            #endregion

            #region Decode

            /// <summary>
            /// Decodes the given base64 string
            /// </summary>
            /// <param name="value">The base64 encoded string of a Guid</param>
            /// <returns>A new Guid</returns>
            public static Guid Decode(string value)
            {
                value = value
                    .Replace("_", "/")
                    .Replace("-", "+");
                byte[] buffer = Convert.FromBase64String(value + "==");
                return new Guid(buffer);
            }

            #endregion

            #region Operators

            /// <summary>
            /// Determines if both ShortGuids have the same underlying
            /// Guid value.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public static bool operator ==(ShortGuid x, ShortGuid y)
            {
                if ((object)x == null) return (object)y == null;
                return x._guid == y._guid;
            }

            /// <summary>
            /// Determines if both ShortGuids do not have the
            /// same underlying Guid value.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public static bool operator !=(ShortGuid x, ShortGuid y)
            {
                return !(x == y);
            }

            /// <summary>
            /// Implicitly converts the ShortGuid to it's string equivilent
            /// </summary>
            /// <param name="shortGuid"></param>
            /// <returns></returns>
            public static implicit operator string(ShortGuid shortGuid)
            {
                return shortGuid._value;
            }

            /// <summary>
            /// Implicitly converts the ShortGuid to it's Guid equivilent
            /// </summary>
            /// <param name="shortGuid"></param>
            /// <returns></returns>
            public static implicit operator Guid(ShortGuid shortGuid)
            {
                return shortGuid._guid;
            }

            /// <summary>
            /// Implicitly converts the string to a ShortGuid
            /// </summary>
            /// <param name="shortGuid"></param>
            /// <returns></returns>
            public static implicit operator ShortGuid(string shortGuid)
            {
                return new ShortGuid(shortGuid);
            }

            /// <summary>
            /// Implicitly converts the Guid to a ShortGuid
            /// </summary>
            /// <param name="guid"></param>
            /// <returns></returns>
            public static implicit operator ShortGuid(Guid guid)
            {
                return new ShortGuid(guid);
            }

            #endregion
        }
    }
}