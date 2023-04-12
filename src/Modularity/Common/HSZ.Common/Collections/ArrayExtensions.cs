using HSZ.Dependency;
using System;

namespace HSZ.Common.Collections
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：数组扩展方法
    /// </summary>
    [SuppressSniffer]
    public static class ArrayExtensions
    {
        /// <summary>
        /// 复制一份二维数组的副本
        /// </summary>
        public static byte[,] Copy(this byte[,] bytes)
        {
            int width = bytes.GetLength(0), height = bytes.GetLength(1);
            byte[,] newBytes = new byte[width, height];
            Array.Copy(bytes, newBytes, bytes.Length);
            return newBytes;
        }
    }
}
