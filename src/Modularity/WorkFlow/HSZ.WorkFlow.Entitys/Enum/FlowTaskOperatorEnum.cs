using HSZ.Dependency;
using System.ComponentModel;

namespace HSZ.WorkFlow.Entitys.Enum
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：经办对象
    /// </summary>
    [SuppressSniffer]
    public enum FlowTaskOperatorEnum
    {
        /// <summary>
        /// 发起者主管
        /// </summary>
        [Description("发起者主管")]
        LaunchCharge = 1,
        /// <summary>
        /// 发起者部门主管
        /// </summary>
        [Description("发起者部门主管")]
        DepartmentCharge = 2,
        /// <summary>
        /// 发起者本人
        /// </summary>
        [Description("发起者本人")]
        InitiatorMe = 3,
        /// <summary>
        /// 获取表单某个值为审批人
        /// </summary>
        [Description("变量")]
        VariableApprover = 4,
        /// <summary>
        /// 之前节点的审批人
        /// </summary>
        [Description("环节")]
        LinkApprover = 5,
        /// <summary>
        /// 候选审批人
        /// </summary>
        [Description("候选审批人")]
        CandidateApprover = 7,
        /// <summary>
        /// 服务（调用指定接口获取数据）
        /// </summary>
        [Description("服务")]
        ServiceApprover = 9,
        /// <summary>
        /// 子流程
        /// </summary>
        [Description("子流程")]
        SubProcesses = 10
    }
}