using HSZ.Dependency;
using System;

namespace HSZ.Common.Extension
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：布尔值<see cref="Boolean"/>类型的扩展辅助操作类
    /// </summary>
    [SuppressSniffer]
    public static class BooleanExtensions
    {
        /// <summary>
        /// 把布尔值转换为小写字符串
        /// </summary>
        public static string ToLower(this bool value)
        {
            return value.ToString().ToLower();
        }

        /// <summary>
        /// 如果条件成立，则抛出异常
        /// </summary>
        public static void TrueThrow(this bool flag, Exception exception)
        {
            if (flag)
            {
                throw exception;
            }
        }
    }
}
