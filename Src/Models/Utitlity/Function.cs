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
using System.Web;
using System.Web.Helpers;
using System.Web.Hosting;

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
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
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
                if (img.ContentLength <= 102400)
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
                    if (temp == Common.ResualtMessage.OK)
                    {
                        DelImg(id, path);
                        img.SaveAs(HostingEnvironment.MapPath($"~/Files/{path}/{id}.jpg"));
                    }
                }
            }
            else
            {
                temp = Common.ResualtMessage.OK;
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
        public static Admin.AInfo GetAdminInfo(HttpRequestBase request)
        {
            Admin.AInfo Item = new Admin.AInfo();
            if (request.Cookies.Count > 0)
            {
                Item.Name = request.Cookies["SSODesc"]["Name"];
                Item.Role = request.Cookies["SSODesc"]["Role"];
                Item.Token = request.Cookies["SSODesc"]["Token"];
            }
            else
            {
                Item.Name = "test";
                Item.Role = "test";
                Item.Token = "a4d5sd455f44==";
            }

            return Item;
        }
        #endregion
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
            using (ALDBEntities db = new ALDBEntities())
            {
                int count = db.Set<T>().Count(exp);
                return count == 0 ? false : true;
            }
        }
    }
}