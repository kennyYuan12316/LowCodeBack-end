using HSZ.Dependency;
using System.ComponentModel;

namespace HSZ.Common.Enum
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：公共状态
    /// </summary>
    [SuppressSniffer]
    public enum CommonStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        ENABLE = 0,

        /// <summary>
        /// 停用
        /// </summary>
        [Description("停用")]
        DISABLE = 1,

        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        DELETED = 2
    }
}
