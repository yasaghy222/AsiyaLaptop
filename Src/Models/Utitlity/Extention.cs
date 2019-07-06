using System;
using System.Linq;
using Newtonsoft.Json;
using Src.Models.Data;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Net.Http;
using System.Text;

namespace Src.Models.Utitlity
{
    public static class EnumExtensions
    {
        /// <summary>
        ///     A generic extension method that aids in reflecting 
        ///     and retrieving any attribute that is applied to an `Enum`.
        /// </summary>
        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
                where TAttribute : Attribute
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<TAttribute>();
        }
        /// <summary>
        /// Parse a string value to the given Enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string value)
        where T : struct
        {
            Debug.Assert(!string.IsNullOrEmpty(value));
            return (T)Enum.Parse(typeof(T), value, true);
        }

        /// <summary>
        /// Converts Enumeration type into a dictionary of names and values
        /// </summary>
        /// <param name="t">Enum type</param>
        public static IDictionary<string, int> EnumToDictionary(this Type t)
        {
            if (t == null) throw new NullReferenceException();
            if (!t.IsEnum) throw new InvalidCastException("object is not an Enumeration");

            string[] names = Enum.GetNames(t);
            Array values = Enum.GetValues(t);

            return (from i in Enumerable.Range(0, names.Length)
                    select new { Key = names[i], Value = (int)values.GetValue(i) })
                        .ToDictionary(k => k.Key, k => k.Value);
        }
        //گرفتن اسم یه اینام از روی ولیو
        // (EnumExtensions.GetEnumValue<PostFormatType>("ولیو!")).GetAttribute<DisplayAttribute>().Name;

        public static class EnumHelper<T>
        {
            /// <summary>
            /// 
            /// Example:
            /// public enum EnumGrades
            ///{
            /// [Description("Passed")]
            /// Pass,
            /// [Description("Failed")]
            /// Failed,
            /// [Description("Promoted")]
            /// Promoted
            ///}
            ///
            /// string description = EnumHelper<![CDATA[<EnumGrades>]]>.GetEnumDescription("pass");
            /// </summary>
            /// <typeparam name="T">Enum Type</typeparam>
            public static string GetEnumDescription(string value)
            {
                Type type = typeof(T);
                var name = Enum.GetNames(type).Where(f => f.Equals(value, StringComparison.CurrentCultureIgnoreCase)).Select(d => d).FirstOrDefault();

                if (name == null)
                {
                    return string.Empty;
                }
                var field = type.GetField(name);
                var customAttribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                return customAttribute.Length > 0 ? ((DescriptionAttribute)customAttribute[0]).Description : name;
            }
        }

        /// 
        /// Method to get enumeration value from string value.
        /// </summary>
        public static T GetEnumValue<T>(string str) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new Exception("T must be an Enumeration type.");
            }
            T val = ((T[])Enum.GetValues(typeof(T)))[0];
            if (!string.IsNullOrEmpty(str))
            {
                foreach (T enumValue in (T[])Enum.GetValues(typeof(T)))
                {
                    if (enumValue.ToString().ToUpper().Equals(str.ToUpper()))
                    {
                        val = enumValue;
                        break;
                    }
                }
            }

            return val;
        }

        ///<summary>
        /// Method to get enumeration value from int value.
        /// </summary>
        public static T GetEnumValue<T>(int intValue) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new Exception("T must be an Enumeration type.");
            }
            T val = ((T[])Enum.GetValues(typeof(T)))[0];

            foreach (T enumValue in (T[])Enum.GetValues(typeof(T)))
            {
                if (Convert.ToInt32(enumValue).Equals(intValue))
                {
                    val = enumValue;
                    break;
                }
            }
            return val;
        }

        ///
        //Example:
        //TestEnum reqValue = GetEnumValue<TestEnum>("Value1");  // Output: Value1
        //TestEnum reqValue2 = GetEnumValue<TestEnum>(2);
        //
    }

    public static class Extensions
    {
        #region general
        /// <summary>
        /// split input by 3
        /// </summary>
        /// <param name="InputText">input</param>
        /// <returns></returns>
        public static string SetCama(this object InputText)
        {
            string num = "0";
            try
            {
                if (InputText == null || string.IsNullOrEmpty(InputText.ToString()))
                    return "0";
                num = InputText.ToString();
                double.TryParse(InputText.ToString(), out double number);
                string res = string.Format("{0:###,###.####}", number);
                string temp = string.IsNullOrEmpty(res) ? "0" : res;
                return temp;
            }
            catch
            {
                return num;
            }
        }

        /// <summary>
        /// deserialize json to c# entity
        /// </summary>
        /// <typeparam name="T">type of output</typeparam>
        /// <param name="value">json input</param>
        /// <returns></returns>
        public static T DeserializeJson<T>(this object value)
        {
            string Temp = JsonConvert.SerializeObject(value);
            T Result = JsonConvert.DeserializeObject<T>(Temp);
            return Result;
        }

        /// <summary>
        /// serialize c# entity to json string
        /// </summary>
        /// <typeparam name="T">T entity type</typeparam>
        /// <param name="entity">c# entity</param>
        /// <returns></returns>
        public static string SerializeJson<T>(this T entity) => JsonConvert.SerializeObject(entity);

        /// <summary>
        /// substring from beginning of the string to custom word position
        /// </summary>
        /// <param name="str">string</param>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string SubStrFirst(this string str, string word) => str.Substring(0, str.IndexOf(word));

        /// <summary>
        /// substring from custom word position to end of the string
        /// </summary>
        /// <param name="str">string</param>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string SubStrLast(this string str, string word) => str.Substring(str.IndexOf(word));
        #endregion

        #region datetime and calender
        /// <summary>
        /// convert datetime to persian date
        /// </summary>
        /// <param name="dateTime">datetime</param>
        /// <param name="format">text: year monthName day, fullText: year monthName day dayOfweek, default: year/month/day</param>
        /// <returns></returns>
        public static string ToPersianDate(this DateTime dateTime, string format = "")
        {
            PersianCalendar pc = new PersianCalendar();
            switch (format)
            {
                case "text": return $"{pc.GetDayOfMonth(dateTime)} {dateTime.GetMonthName()} {pc.GetYear(dateTime)}";
                case "fullText": return $"{dateTime.GetDayOfWeekName()} {pc.GetDayOfMonth(dateTime)} {dateTime.GetMonthName()} {pc.GetYear(dateTime)}";
                case "short": string temp = pc.GetYear(dateTime).ToString(); return $"{temp.Substring(2)}{pc.GetMonth(dateTime)}{pc.GetDayOfMonth(dateTime)}";
                default: return $"{pc.GetYear(dateTime)}/{pc.GetMonth(dateTime)}/{pc.GetDayOfMonth(dateTime)}";
            }
        }
        /// <summary>
        /// convert datetime to persian date
        /// </summary>
        /// <param name="dateTime">datetime</param>
        /// <param name="format">text: year monthName day, fullText: year monthName day dayOfweek, default: year/month/day</param>
        /// <returns></returns>
        public static string ToPersianDate(this DateTime? dateTime, string format)
        {
            PersianCalendar pc = new PersianCalendar();
            DateTime dt = (DateTime)dateTime;
            switch (format)
            {
                case "text": return $"{pc.GetDayOfMonth(dt)} {dt.GetMonthName()} {pc.GetYear(dt)}";
                case "fullText": return $"{dt.GetDayOfWeekName()} {pc.GetDayOfMonth(dt)} {dt.GetMonthName()} {pc.GetYear(dt)}";
                default: return $"{pc.GetYear(dt)}/{pc.GetMonth(dt)}/{pc.GetDayOfMonth(dt)}";
            }
        }

        /// <summary>
        /// convert persian date to datetime
        /// </summary>
        /// <param name="persianDate"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string persianDate)
        {
            PersianCalendar pc = new PersianCalendar();
            try
            {
                string[] date = persianDate.Split('/');
                int year = int.Parse(date[0]),
                    month = int.Parse(date[1]),
                    day = int.Parse(date[2]);

                DateTime dateTime = pc.ToDateTime(year, month, day, 0, 0, 0, 0, PersianCalendar.PersianEra);

                return dateTime;
            }
            catch
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// get month in persian calender
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string GetMonthName(this DateTime date)
        {
            PersianCalendar pc = new PersianCalendar();
            string pdate = $"{pc.GetYear(date)}/{pc.GetMonth(date)}/{pc.GetDayOfMonth(date)}";

            string[] dates = pdate.Split('/');
            int month = int.Parse(dates[1]);

            switch (month)
            {
                case 1: return "فررودين";
                case 2: return "ارديبهشت";
                case 3: return "خرداد";
                case 4: return "تير‏";
                case 5: return "مرداد";
                case 6: return "شهريور";
                case 7: return "مهر";
                case 8: return "آبان";
                case 9: return "آذر";
                case 10: return "دي";
                case 11: return "بهمن";
                case 12: return "اسفند";
                default: return "";
            }

        }

        /// <summary>
        /// get day of week in persian calender
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string GetDayOfWeekName(this DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Saturday: return "شنبه";
                case DayOfWeek.Sunday: return "يکشنبه";
                case DayOfWeek.Monday: return "دوشنبه";
                case DayOfWeek.Tuesday: return "سه‏ شنبه";
                case DayOfWeek.Wednesday: return "چهارشنبه";
                case DayOfWeek.Thursday: return "پنجشنبه";
                case DayOfWeek.Friday: return "جمعه";
                default: return "";
            }
        }
        #endregion

        #region project
        /// <summary>
        /// return product category backtrack 
        /// </summary>
        /// <param name="item">procCat</param>
        /// <returns></returns>
        public static string GetCatList(this Tbl_ProcCat item)
        {
            string temp = string.Empty;
            if (item.PID == null)
            {
                temp = $"لپتاپ آسیا - {item.Title}";
            }
            else
            {
                using (ALDBEntities db = new ALDBEntities())
                {
                    Tbl_ProcCat parent = db.Tbl_ProcCat.Single(x => x.ID == item.PID);
                    if (parent.PID == null)
                    {
                        temp = $"لپتاپ آسیا - {parent.Title} - {item.Title}";
                    }
                    else
                    {
                        temp = $"{parent.GetCatList()} - {item.Title}";
                    }
                }
            }
            return temp;
        }
        #endregion
    }
}