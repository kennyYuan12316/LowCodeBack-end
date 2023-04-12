using HSZ.Common.Const;
using HSZ.Common.Entity;
using SqlSugar;
using System;

namespace HSZ.WorkFlow.Entitys
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：流程经办
    /// </summary>
    [SugarTable("ZJN_FLOW_TASK_OPERATOR")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class FlowTaskOperatorEntity: EntityBase<string>
    {
        /// <summary>
        /// 经办对象
        /// </summary>
        [SugarColumn(ColumnName = "F_HANDLETYPE")]
        public string HandleType { get; set; }
        /// <summary>
        /// 经办主键
        /// </summary>
        [SugarColumn(ColumnName = "F_HANDLEID")]
        public string HandleId { get; set; }
        /// <summary>
        /// 处理状态：【0-拒绝、1-同意】
        /// </summary>
        [SugarColumn(ColumnName = "F_HANDLESTATUS")]
        public int? HandleStatus { get; set; }
        /// <summary>
        /// 处理时间
        /// </summary>
        [SugarColumn(ColumnName = "F_HANDLETIME")]
        public DateTime? HandleTime { get; set; }
        /// <summary>
        /// 节点编码
        /// </summary>
        [SugarColumn(ColumnName = "F_NODECODE")]
        public string NodeCode { get; set; }
        /// <summary>
        /// 节点名称
        /// </summary>
        [SugarColumn(ColumnName = "F_NODENAME")]
        public string NodeName { get; set; }
        /// <summary>
        /// 是否完成
        /// </summary>
        [SugarColumn(ColumnName = "F_COMPLETION")]
        public int? Completion { get; set; }
        /// <summary>
        /// 描述(超时时间)
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CREATORTIME")]
        public DateTime? CreatorTime { get; set; }
        /// <summary>
        /// 节点主键
        /// </summary>
        [SugarColumn(ColumnName = "F_TASKNODEID")]
        public string TaskNodeId { get; set; }
        /// <summary>
        /// 任务主键
        /// </summary>
        [SugarColumn(ColumnName = "F_TASKID")]
        public string TaskId { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        [SugarColumn(ColumnName = "F_TYPE")]
        public string Type { get; set; }
        /// <summary>
        /// 审批状态(-1:审批拒绝)
        /// </summary>
        [SugarColumn(ColumnName = "F_STATE")]
        public string State { get; set; }
        /// <summary>
        /// 加签人
        /// </summary>
        [SugarColumn(ColumnName = "F_PARENTID")]
        public string ParentId { get; set; }
        /// <summary>
        /// 保存数据
        /// </summary>
        [SugarColumn(ColumnName = "F_DRAFTDATA")]
        public string DraftData { get; set; }
    }
}