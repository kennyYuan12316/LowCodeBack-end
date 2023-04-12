using HSZ.Dependency;
using System.IO;
using System.Text;

namespace HSZ.Common.Extension
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：Stream扩展方法
    /// </summary>
    [SuppressSniffer]
    public static class StreamExtensions
    {
        /// <summary>
        /// 把<see cref="Stream"/>转换为<see cref="string"/>
        /// </summary>
        public static string ToString2(this Stream stream, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            using (StreamReader reader = new StreamReader(stream, encoding))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
