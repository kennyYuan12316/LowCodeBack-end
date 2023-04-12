using HSZ.Dependency;
using System.ComponentModel;

namespace HSZ.Common.Enum
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：通知公告状态
    /// </summary>
    [SuppressSniffer]
    public enum NoticeStatus
    {
        /// <summary>
        /// 草稿
        /// </summary>
        [Description("草稿")]
        DRAFT = 0,

        /// <summary>
        /// 发布
        /// </summary>
        [Description("发布")]
        PUBLIC = 1,

        /// <summary>
        /// 撤回
        /// </summary>
        [Description("撤回")]
        CANCEL = 2,

        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        DELETED = 3
    }
}
