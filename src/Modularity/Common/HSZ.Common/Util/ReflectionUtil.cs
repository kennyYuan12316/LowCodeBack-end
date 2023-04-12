using HSZ.Dependency;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace HSZ.Common.Util
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：反射工具
    /// </summary>
    [SuppressSniffer]
    public static class ReflectionUtil
    {
        /// <summary>
        /// 获取字段特性
        /// </summary>
        /// <param name="field"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetDescriptionValue<T>(this FieldInfo field) where T : Attribute
        {
            // 获取字段的指定特性，不包含继承中的特性
            object[] customAttributes = field.GetCustomAttributes(typeof(T), false);

            // 如果没有数据返回null
            return customAttributes.Length > 0 ? (T)customAttributes[0] : null;
        }
    }
}
