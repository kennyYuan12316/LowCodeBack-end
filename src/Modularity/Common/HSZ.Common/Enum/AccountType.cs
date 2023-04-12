using HSZ.Dependency;
using System.ComponentModel;

namespace HSZ.Common.Enum
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：账号类型
    /// </summary>
    [SuppressSniffer]
    public enum AccountType
    {
        /// <summary>
        /// 普通账号
        /// </summary>
        [Description("普通账号")]
        None = 0,

        /// <summary>
        /// 超级管理员
        /// </summary>
        [Description("超级管理员")]
        Administrator = 1,
    }
}
