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

        public static string GetPersianDate(this DateTime? date)
        {
            DateTime d = (DateTime)date;
            PersianCalendar jc = new PersianCalendar();
            return string.Format("{0:0000}/{1:00}/{2:00}", jc.GetYear(d), jc.GetMonth(d), jc.GetDayOfMonth(d));
        }

        public static string GetPersianDate(this DateTime date)
        {
            PersianCalendar jc = new PersianCalendar();
            return string.Format("{0:0000}/{1:00}/{2:00}", jc.GetYear(date), jc.GetMonth(date), jc.GetDayOfMonth(date));
        }

        public static string GetShortPersianDate(this DateTime date)
        {
            PersianCalendar jc = new PersianCalendar();
            string temp = jc.GetYear(date).ToString();
            return $"{temp.Substring(2)}{jc.GetMonth(date)}{jc.GetDayOfMonth(date)}";
        }

        public static string GetPersianDate(this DateTime? date, int type)
        {
            string temp = "";
            DateTime d = (DateTime)date;
            PersianCalendar jc = new PersianCalendar();
            switch (type)
            {
                case 1:
                    temp = $"{d.GetDayOfWeekName()} {jc.GetDayOfMonth(d)} {d.GetMonthName()}";
                    break;

                case 2:
                    temp = $"{d.GetDayOfWeekName()} {jc.GetDayOfMonth(d)} {d.GetMonthName()} {jc.GetYear(d)}";
                    break;
                case 3:
                    temp = $"{jc.GetYear(d)}/{jc.GetMonth(d)}/{jc.GetDayOfMonth(d)} {d.ToString("HH:MM")}";
                    break;
                default:
                    temp = string.Format("{0:0000}/{1:00}/{2:00}", jc.GetYear(d), jc.GetMonth(d), jc.GetDayOfMonth(d));
                    break;
            }
            return temp;
        }

        public static string GetPersianDate(this DateTime date, int type)
        {
            string temp = "";
            PersianCalendar jc = new PersianCalendar();
            switch (type)
            {
                case 1:
                    temp = $"{date.GetDayOfWeekName()} {jc.GetDayOfMonth(date)} {date.GetMonthName()}";
                    break;

                case 2:
                    temp = $"{date.GetDayOfWeekName()} {jc.GetDayOfMonth(date)} {date.GetMonthName()} {jc.GetYear(date)}";
                    break;
                case 3:
                    temp = $"{jc.GetYear(date)}/{jc.GetMonth(date)}/{jc.GetDayOfMonth(date)} {date.ToString("HH:MM")}";
                    break;
                default:
                    temp = string.Format("{0:0000}/{1:00}/{2:00}", jc.GetYear(date), jc.GetMonth(date), jc.GetDayOfMonth(date));
                    break;
            }
            return temp;
        }

        public static DateTime GetDateTimeFromJalaliString(this string jalaliDate)
        {
            PersianCalendar jc = new PersianCalendar();

            try
            {
                string[] date = jalaliDate.Split('/');
                int year = Convert.ToInt32(date[0]);
                int month = Convert.ToInt32(date[1]);
                int day = Convert.ToInt32(date[2]);

                DateTime d = jc.ToDateTime(year, month, day, 0, 0, 0, 0, PersianCalendar.PersianEra);

                return d;
            }
            catch
            {
                return DateTime.Now;
            }
        }

        public static string GetMonthName(this DateTime date)
        {
            PersianCalendar jc = new PersianCalendar();
            string pdate = string.Format("{0:0000}/{1:00}/{2:00}", jc.GetYear(date), jc.GetMonth(date), jc.GetDayOfMonth(date));

            string[] dates = pdate.Split('/');
            int month = Convert.ToInt32(dates[1]);

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

        public static T ToJson<T>(this object value)
        {
            string Temp = JsonConvert.SerializeObject(value);
            T Resualt = JsonConvert.DeserializeObject<T>(Temp);
            return Resualt;
        }
    }
}