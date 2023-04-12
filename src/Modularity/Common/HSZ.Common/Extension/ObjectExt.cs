using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using HSZ.Common.Extension.Extensions;

namespace HSZ.Common.Extension
{
    public static class ObjectExt
    {
        /// <summary>
        /// 对象的字段值非空验证
        /// </summary>
        public static bool ObjectIsNullOrEmpty<T>(this T t, string[] strArr)
        {
            var collection = t.GetType().GetProperties().Where(l => !strArr.Contains(l.Name));
            foreach (var item in collection)
            {
                var value = item.GetValue(t);
                if (value.IsEmpty())
                    throw new Exception($"字段:{item.Name}不允许为空");
            }
            return false;
        }

        public static bool ObjectIsNullOrEmpty<T>(this T t)
        {
            var collection = t.GetType().GetProperties();
            foreach (var item in collection)
            {
                var value = item.GetValue(t);
                //过滤忽略字段
                var requiredAttribute = item.GetCustomAttribute<RequiredAttribute>();
                if (requiredAttribute != null && requiredAttribute.AllowEmptyStrings)
                    continue;

                if (value.IsEmpty())
                    throw new Exception($"字段:{item.Name}不允许为空");
            }
            return false;
        }
      
        /// <summary>
        /// List的字段值非空验证
        /// </summary>
        public static bool ObjectIsNullOrEmpty<T>(this List<T> list, string[] strArr, LamadaExt<T> ext)
        {
            if (list.Count() == 0)
                return false;
            T t = default(T);
            t = Activator.CreateInstance<T>();
            var collection = t.GetType().GetProperties().Where(l => !strArr.Contains(l.Name)).Select(l=>l.Name).ToList();
            ext.GetOrExpression(collection);
            var lamada = ext.GetLambda().Compile();
            return list.Any(lamada);
        }

        /// <summary>
        /// List的字段值非空验证
        /// </summary>
        public static bool AreAnyDuplicates<T>(this IEnumerable<T> list)
        {
            var hashset = new HashSet<T>();
            return list.Any(l=>!hashset.Add(l));
        }

        /// <summary>
        /// 比较两个对象的属性值和字段值是否全部相等
        /// </summary>
        public static bool Compare<T>(T obj1, T obj2, Type type)
        {
            return CompareProperties(obj1, obj2, type) && CompareFields(obj1, obj2, type);
        }

        /// <summary>
        /// 判断两个相同引用类型的对象的属性值是否相等
        /// </summary>
        public static bool CompareProperties<T>(T obj1, T obj2, Type type)
        {
            //为空判断
            if (obj1 == null && obj2 == null)
                return true;
            else if (obj1 == null || obj2 == null)
                return false;

            Type t = type;

            PropertyInfo[] props = t.GetProperties();
            foreach (var po in props)
            {
                if (IsCanCompare(po.PropertyType))
                {
                    var befour = po.GetValue(obj1).ToString();
                    var after = po.GetValue(obj2).ToString();
                    if (!befour.Equals(after))
                        return false;
                }
                else
                {
                    var b = CompareProperties(po.GetValue(obj1), po.GetValue(obj2),po.PropertyType);
                    if (!b) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 判断两个相同引用类型的对象的字段值是否相等
        /// </summary>
        public static bool CompareFields<T>(T obj1, T obj2, Type type)
        {
            //为空判断
            if (obj1 == null && obj2 == null)
                return true;
            else if (obj1 == null || obj2 == null)
                return false;

            Type t = type;

            FieldInfo[] fields = t.GetFields();
            foreach (var fd in fields)
            {
                if (IsCanCompare(fd.FieldType))
                {
                    if (!(fd.GetValue(obj1) == fd.GetValue(obj2)))
                        return false;
                }
                else
                {
                    if (!CompareFields(fd.GetValue(obj1), fd.GetValue(obj2),fd.FieldType))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 该类型是否可直接进行值的比较
        /// </summary>
        private static bool IsCanCompare(Type t)
        {
            if (t.IsValueType)
                return true;
            else
            {
                //String是特殊的引用类型，它可以直接进行值的比较
                if (t.FullName == typeof(String).FullName)
                    return true;
                return false;
            }
        }

    }
}
