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
    /// 描 述：流程经办记录
    /// </summary>
    [SugarTable("ZJN_FLOW_TASK_OPERATOR_RECORD")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class FlowTaskOperatorRecordEntity : EntityBase<string>
    {
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
        /// 经办状态：【0-拒绝、1-同意、2-提交、3-撤回、4-终止】
        /// </summary>
        [SugarColumn(ColumnName = "F_HANDLESTATUS")]
        public int HandleStatus { get; set; }=0;
        /// <summary>
        /// 经办人员
        /// </summary>
        [SugarColumn(ColumnName = "F_HANDLEID")]
        public string HandleId { get; set; }
        /// <summary>
        /// 经办时间
        /// </summary>
        [SugarColumn(ColumnName = "F_HANDLETIME")]
        public DateTime? HandleTime { get; set; }
        /// <summary>
        /// 经办理由
        /// </summary>
        [SugarColumn(ColumnName = "F_HANDLEOPINION")]
        public string HandleOpinion { get; set; }
        /// <summary>
        /// 经办主键
        /// </summary>
        [SugarColumn(ColumnName = "F_TASKOPERATORID")]
        public string TaskOperatorId { get; set; }
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
        /// 电子签名
        /// </summary>
        [SugarColumn(ColumnName = "F_SIGNIMG")]
        public string SignImg { get; set; }
        /// <summary>
        /// 审批标识(1:加签人)
        /// </summary>
        [SugarColumn(ColumnName = "F_STATUS")]
        public int? Status { get; set; }
        /// <summary>
        /// 流转操作人
        /// </summary>
        [SugarColumn(ColumnName = "F_OPERATORID")]
        public string OperatorId { get; set; }
    }
}