using S7.Net.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Zjn.Siem
{
    public static class Common
    {
        /// <summary>
        /// Json序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string Serializer<T>(T obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// Json反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string jsonString)
        {

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonString);

        }

        /// <summary>
        /// 此方法意义不大，暂时搁置。尤其bit,要先捞正byte,再合，麻烦
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="obj"></param>
        /// <param name="expression"></param>
        public static void PlcWrite<T, TProperty>(this T obj, Expression<Func<T, TProperty>> expression)
        {
            MemberExpression memberExpression = expression.Body as MemberExpression;
            var member = memberExpression.Member;

            //属性的类型
            string type = member.DeclaringType.FullName;
            //属性的名称
            string name = member.Name;
            var property = typeof(T).GetProperties().Where(x => x.Name == name).FirstOrDefault();
            //属性的值
            var value = property.GetValue(obj);
            //特征的起始位 (对象中起始位）
            var start = property.GetCustomAttribute<PackStartAttribute>().PackStart;
            //获取db
            var p1 = typeof(T).GetProperties().Where(x => x.Name == "Db").FirstOrDefault();
            int db = (int)p1.GetValue(obj);
            //获取Plc
            var p2 = typeof(T).GetProperties().Where(x => x.Name == "Plc").FirstOrDefault();
            Device plc = (Device)p2.GetValue(obj);
            //把值转换成bytes

            /////方法不对，非对象。bit,无法装载到所在byte中
            //var bytes = plc.GetClassToBytes(value);


            //var p3 = typeof(T).GetProperties().Where(x => x.Name == "Start").FirstOrDefault();
            //start = +(int)p3.GetValue(obj);


            //plc.WriteDbBytes(db, start, bytes);
        }


    }
}
