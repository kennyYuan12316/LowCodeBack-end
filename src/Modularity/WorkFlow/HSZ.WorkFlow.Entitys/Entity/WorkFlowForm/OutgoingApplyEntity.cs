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
    /// 描 述：外出申请单
    /// </summary>
    [SugarTable("ZJN_WFORM_OUTGOING_APPLY")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class OutgoingApplyEntity : EntityBase<string>
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
        /// 申请人
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYUSER")]
        public string ApplyUser { get; set; }
        /// <summary>
        /// 所在部门
        /// </summary>
        [SugarColumn(ColumnName = "F_DEPARTMENT")]
        public string Department { get; set; }
        /// <summary>
        /// 申请日期
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYDATE")]
        public DateTime? ApplyDate { get; set; }
        /// <summary>
        /// 外出总计
        /// </summary>
        [SugarColumn(ColumnName = "F_OUTGOINGTOTLE")]
        public string OutgoingTotle { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [SugarColumn(ColumnName = "F_STARTTIME")]
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        [SugarColumn(ColumnName = "F_ENDTIME")]
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 目的地
        /// </summary>
        [SugarColumn(ColumnName = "F_DESTINATION")]
        public string Destination { get; set; }
        /// <summary>
        /// 相关附件
        /// </summary>
        [SugarColumn(ColumnName = "F_FILEJSON")]
        public string FileJson { get; set; }
        /// <summary>
        /// 外出事由
        /// </summary>
        [SugarColumn(ColumnName = "F_OUTGOINGCAUSE")]
        public string OutgoingCause { get; set; }
    }
}
