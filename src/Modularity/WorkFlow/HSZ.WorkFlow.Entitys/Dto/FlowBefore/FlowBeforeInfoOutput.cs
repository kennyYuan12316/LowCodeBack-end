using HSZ.Dependency;
using HSZ.WorkFlow.Entitys.Model;
using HSZ.WorkFlow.Entitys.Model.Properties;
using System.Collections.Generic;

namespace HSZ.WorkFlow.Entitys.Dto.FlowBefore
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class FlowBeforeInfoOutput
    {
        /// <summary>
        /// 表单json
        /// </summary>
        public string flowFormInfo { get; set; }
        /// <summary>
        /// 流程任务
        /// </summary>
        public FlowTaskModel flowTaskInfo { get; set; }
        /// <summary>
        /// 流程任务节点
        /// </summary>
        public List<FlowTaskNodeModel> flowTaskNodeList { get; set; }
        /// <summary>
        /// 流程任务经办
        /// </summary>
        public List<FlowTaskOperatorModel> flowTaskOperatorList { get; set; }
        /// <summary>
        /// 流程任务经办记录
        /// </summary>
        public List<FlowTaskOperatorRecordModel> flowTaskOperatorRecordList { get; set; }
        /// <summary>
        /// 当前节点权限
        /// </summary>
        public List<FormOperatesModel> formOperates { get; set; } = new List<FormOperatesModel>();
        /// <summary>
        /// 当前节点属性
        /// </summary>
        public ApproversProperties approversProperties { get; set; } = new ApproversProperties();
        /// <summary>
        /// 审核保存数据
        /// </summary>
        public object draftData { get; set; }
    }
}
