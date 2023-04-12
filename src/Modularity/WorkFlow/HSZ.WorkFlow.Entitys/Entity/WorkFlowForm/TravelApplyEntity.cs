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
    /// 描 述：出差预支申请单
    /// </summary>
    [SugarTable("ZJN_WFORM_TRAVEL_APPLY")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class TravelApplyEntity : EntityBase<string>
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
        /// 出差人
        /// </summary>
        [SugarColumn(ColumnName = "F_TRAVELMAN")]
        public string TravelMan { get; set; }
        /// <summary>
        /// 申请日期
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYDATE")]
        public DateTime? ApplyDate { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        [SugarColumn(ColumnName = "F_DEPARTMENTAL")]
        public string Departmental { get; set; }
        /// <summary>
        /// 所属职务
        /// </summary>
        [SugarColumn(ColumnName = "F_POSITION")]
        public string Position { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        [SugarColumn(ColumnName = "F_STARTDATE")]
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        [SugarColumn(ColumnName = "F_ENDDATE")]
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 起始地点
        /// </summary>
        [SugarColumn(ColumnName = "F_STARTPLACE")]
        public string StartPlace { get; set; }
        /// <summary>
        /// 目的地
        /// </summary>
        [SugarColumn(ColumnName = "F_DESTINATION")]
        public string Destination { get; set; }
        /// <summary>
        /// 预支旅费
        /// </summary>
        [SugarColumn(ColumnName = "F_PREPAIDTRAVEL")]
        public decimal? PrepaidTravel { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
    }
}
