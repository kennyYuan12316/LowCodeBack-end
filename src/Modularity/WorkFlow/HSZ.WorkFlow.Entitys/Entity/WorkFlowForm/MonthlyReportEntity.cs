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
    /// 描 述：月工作总结
    /// </summary>
    [SugarTable("ZJN_WFORM_MONTHLY_REPORT")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class MonthlyReportEntity : EntityBase<string>
    {
        /// <summary>
        /// 流程主键
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWID")]
        public string FlowId { get; set; }
        /// <summary>
        /// 流程标题
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWTITLE")]
        public string FlowTitle { get; set; }
        /// <summary>
        /// 紧急程度
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWURGENT")]
        public int? FlowUrgent { get; set; }
        /// <summary>
        /// 流程单据
        /// </summary>
        [SugarColumn(ColumnName = "F_BILLNO")]
        public string BillNo { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYUSER")]
        public string ApplyUser { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYDATE")]
        public DateTime? ApplyDate { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYDEPT")]
        public string ApplyDept { get; set; }
        /// <summary>
        /// 所属职务
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYPOST")]
        public string ApplyPost { get; set; }
        /// <summary>
        /// 完成时间
        /// </summary>
        [SugarColumn(ColumnName = "F_PLANENDTIME")]
        public DateTime? PlanEndTime { get; set; }
        /// <summary>
        /// 总体评价
        /// </summary>
        [SugarColumn(ColumnName = "F_OVERALEVALUAT")]
        public string OveralEvaluat { get; set; }
        /// <summary>
        /// 工作事项
        /// </summary>
        [SugarColumn(ColumnName = "F_NPWORKMATTER")]
        public string NPWorkMatter { get; set; }
        /// <summary>
        /// 次月日期
        /// </summary>
        [SugarColumn(ColumnName = "F_NPFINISHTIME")]
        public DateTime? NPFinishTime { get; set; }
        /// <summary>
        /// 次月目标
        /// </summary>
        [SugarColumn(ColumnName = "F_NFINISHMETHOD")]
        public string NFinishMethod { get; set; }
        /// <summary>
        /// 相关附件
        /// </summary>
        [SugarColumn(ColumnName = "F_FILEJSON")]
        public string FileJson { get; set; }
    }
}
