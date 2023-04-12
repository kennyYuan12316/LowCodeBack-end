using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace HSZ.Common.Extension

{
    /// <summary>
    /// String扩展
    /// </summary>
    public static class StringExt
    {
        /// <summary>
        /// 是null还是string.Empty
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNull(this string str)
        {
            return string.IsNullOrEmpty(str);
        }



        public static bool IsEmptyZero(this string str)
        {
            return string.IsNullOrWhiteSpace(str) || str == "0";
        }

        public static bool IsNull2(this string value)
        {
            return value == null || value == "" || value == string.Empty || value == " " || value.Length == 0;
        }



        /// <summary>
        /// 判断是不是为null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsNullT<T>(this T t) where T : class
        {
            return t == null;
        }

        public static bool IsNullOrEmpty<T>(this ICollection<T> source)
        {
            //return source == null || source.Count <= 0;
            return source.Any();
        }

        /// <summary>
        /// 判断List<T>是不是为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsNullT<T>(this List<T> t) where T : class
        {
            return t == null || t.Count == 0;
        }

        public static bool IsNullLt<T>(this List<T> t)
        {
            return t == null || t.Count == 0;
        }

        public static bool IsNullDt(this DataTable dt)
        {
            return dt == null || dt.Rows.Count == 0;
        }

        public static bool IsNullT<T>(this IEnumerable<T> value)
        {
            if (value == null)
                return true;
            return !value.Any();
        }

        public static bool IsNull<T>(this T t) where T : class
        {
            if (t == null)
            {
                return true;
            }
            if (t is string)
            {
                return string.IsNullOrWhiteSpace(t.ToString().Trim());
            }
            if (t is DBNull)
            {
                return true;
            }
            if (t.GetType() == typeof(DataTable))
            {
                Type entityType = typeof(T);
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);
                DataTable dt = new DataTable();
                foreach (PropertyDescriptor prop in properties)
                {
                    dt.Columns.Add(prop.Name);
                }
                return dt == null || dt.Rows.Count == 0;
            }
            return false;
        }

        public static string[] ToSplit(this object obj, char c = '|')
        {
            return obj.ToString().Split(c);
        }

        /// <summary>
        /// 首字母大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToFirstUpper(this string str)
        {
            if (str.IsEmpty())
            {
                throw new ArgumentNullException(nameof(str));
            }
            return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(str);
        }
        public static string GetCharsAt(byte[] Buffer, int Pos, int Size)
        {
            return Encoding.UTF8.GetString(Buffer, Pos, Size);
        }
        public static int GetSIntAt(byte[] Buffer, int Pos)
        {
            int Value = Buffer[Pos];
            if (Value < 128)
                return Value;
            else
                return (int)(Value - 256);
        }

        public static void SetCharsAt(byte[] Buffer, int Pos, string Value)
        {
            int MaxLen = Buffer.Length - Pos;
            // Truncs the string if there's no room enough        
            if (MaxLen > Value.Length) MaxLen = Value.Length;
            Encoding.UTF8.GetBytes(Value, 0, MaxLen, Buffer, Pos);
        }

        /// <summary>
        /// 字符串的长度
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="flag">默认字符集是UTF8,</param>
        /// <returns></returns>
        public static int LengthH(this string str, EncodingType type = EncodingType.UTF8)
        {
            return str.ToBytes(type).Length;
        }

        /// <summary>
        ///是否包含或全部是中文
        /// </summary>
        /// <param name="str"></param>
        /// <param name="match">true全中文，false含有中文</param>
        /// <returns></returns>
        public static bool IsChinese(this string str, bool match = false)
        {
            var bytes = str.ToBytes(EncodingType.gb2312);
            return match ? bytes.Length == str.Length * 2 : bytes.Length > str.Length;
        }

        public static bool IsMatch(this string value, string pattern, RegexOptions options)
        {
            return value != null && Regex.IsMatch(value, pattern, options);
        }

        public static bool IsMatch(this string value, string pattern)
        {
            return value != null && Regex.IsMatch(value, pattern);
        }

        /// <summary>
        /// 转化为日期格式
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static DateTime ToDateTimeFormat(this string src, string format)
        {
            DateTime dt;
            try
            {
                dt = DateTime.ParseExact(src, format, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                dt = new DateTime(1900, 1, 1);
            }
            return dt;
        }
    }
}