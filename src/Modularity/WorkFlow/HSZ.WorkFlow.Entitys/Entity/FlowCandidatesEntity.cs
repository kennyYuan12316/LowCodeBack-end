using HSZ.Common.Const;
using HSZ.Common.Entity;
using SqlSugar;

namespace HSZ.WorkFlow.Entitys
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：流程候选人
    /// </summary>
    [SugarTable("ZJN_FLOW_CANDIDATE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class FlowCandidatesEntity: EntityBase<string>
    {
        /// <summary>
        /// 任务id
        /// </summary>
        [SugarColumn(ColumnName = "F_TASKID")]
        public string TaskId { get; set; }
        /// <summary>
        /// 节点id
        /// </summary>
        [SugarColumn(ColumnName = "F_TASKNODEID")]
        public string TaskNodeId { get; set; }
        /// <summary>
        /// 审批人id
        /// </summary>
        [SugarColumn(ColumnName = "F_HANDLEID")]
        public string HandleId { get; set; }
        /// <summary>
        /// 审批人账号
        /// </summary>
        [SugarColumn(ColumnName = "F_ACCOUNT")]
        public string Account { get; set; }
        /// <summary>
        /// 候选人
        /// </summary>
        [SugarColumn(ColumnName = "F_CANDIDATES")]
        public string Candidates { get; set; }
        /// <summary>
        /// 经办id
        /// </summary>
        [SugarColumn(ColumnName = "F_TASKOPERATORID")]
        public string TaskOperatorId { get; set; }
    }
}
