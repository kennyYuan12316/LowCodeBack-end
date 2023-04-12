using HSZ.Dependency;
using System.ComponentModel;

namespace HSZ.WorkFlow.Entitys.Enum
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public enum FlowMessageEnum
    {
        /// <summary>
        /// 发起
        /// </summary>
        [Description("发起")]
        me = 1,
        /// <summary>
        /// 代办
        /// </summary>
        [Description("代办")]
        wait = 2,
        /// <summary>
        /// 抄送
        /// </summary>
        [Description("抄送")]
        circulate = 3
    }
}
