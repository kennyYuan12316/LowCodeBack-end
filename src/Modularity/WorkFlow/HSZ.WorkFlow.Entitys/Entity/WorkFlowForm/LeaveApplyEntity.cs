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
    /// 描 述：请假申请
    /// </summary>
    [SugarTable("ZJN_WFORM_LEAVE_APPLY")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class LeaveApplyEntity : EntityBase<string>
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
        /// 单据编码
        /// </summary>
        [SugarColumn(ColumnName = "F_BILLNO")]
        public string BillNo { get; set; }
        /// <summary>
        /// 申请人员
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYUSER")]
        public string ApplyUser { get; set; }
        /// <summary>
        /// 申请日期
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYDATE")]
        public DateTime? ApplyDate { get; set; }
        /// <summary>
        /// 申请部门
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYDEPT")]
        public string ApplyDept { get; set; }
        /// <summary>
        /// 申请职位
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYPOST")]
        public string ApplyPost { get; set; }
        /// <summary>
        /// 请假类别
        /// </summary>
        [SugarColumn(ColumnName = "F_LEAVETYPE")]
        public string LeaveType { get; set; }
        /// <summary>
        /// 请假原因
        /// </summary>
        [SugarColumn(ColumnName = "F_LEAVEREASON")]
        public string LeaveReason { get; set; }
        /// <summary>
        /// 请假时间
        /// </summary>
        [SugarColumn(ColumnName = "F_LEAVESTARTTIME")]
        public DateTime? LeaveStartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        [SugarColumn(ColumnName = "F_LEAVEENDTIME")]
        public DateTime? LeaveEndTime { get; set; }
        /// <summary>
        /// 请假天数
        /// </summary>
        [SugarColumn(ColumnName = "F_LEAVEDAYCOUNT")]
        public string LeaveDayCount { get; set; }
        /// <summary>
        /// 请假小时
        /// </summary>
        [SugarColumn(ColumnName = "F_LEAVEHOUR")]
        public string LeaveHour { get; set; }
        /// <summary>
        /// 相关附件
        /// </summary>
        [SugarColumn(ColumnName = "F_FILEJSON")]
        public string FileJson { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
    }
}
