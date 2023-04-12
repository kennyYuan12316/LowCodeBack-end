using HSZ.Dependency;
using System.ComponentModel;

namespace HSZ.Common.Enum
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：文件存储位置
    /// </summary>
    [SuppressSniffer]
    public enum FileLocation
    {
        /// <summary>
        /// 阿里云
        /// </summary>
        [Description("阿里云")]
        ALIYUN = 1,

        /// <summary>
        /// 腾讯云
        /// </summary>
        [Description("腾讯云")]
        TENCENT = 2,

        /// <summary>
        /// minio服务器
        /// </summary>
        [Description("minio服务器")]
        MINIO = 3,

        /// <summary>
        /// 本地
        /// </summary>
        [Description("本地")]
        LOCAL = 4
    }
}
