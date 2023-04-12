using HSZ.Dependency;
using System.ComponentModel;

namespace HSZ.Common.Enum
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：通知公告用户状态
    /// </summary>
    [SuppressSniffer]
    public enum NoticeUserStatus
    {
        /// <summary>
        /// 未读
        /// </summary>
        [Description("未读")]
        UNREAD = 0,

        /// <summary>
        /// 已读
        /// </summary>
        [Description("已读")]
        READ = 1
    }
}
