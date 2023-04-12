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
    /// 描 述：宴请申请
    /// </summary>
    [SugarTable("ZJN_WFORM_BANQUET")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ApplyBanquetEntity : EntityBase<string>
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
        /// 申请人员
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYUSER")]
        public string ApplyUser { get; set; }
        /// <summary>
        /// 所属职务
        /// </summary>
        [SugarColumn(ColumnName = "F_POSITION")]
        public string Position { get; set; }
        /// <summary>
        /// 申请日期
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYDATE")]
        public DateTime? ApplyDate { get; set; }
        /// <summary>
        /// 宴请人数
        /// </summary>
        [SugarColumn(ColumnName = "F_BANQUETNUM")]
        public string BanquetNum { get; set; }
        /// <summary>
        /// 宴请人员
        /// </summary>
        [SugarColumn(ColumnName = "F_BANQUETPEOPLE")]
        public string BanquetPeople { get; set; }
        /// <summary>
        /// 人员总数
        /// </summary>
        [SugarColumn(ColumnName = "F_TOTAL")]
        public string Total { get; set; }
        /// <summary>
        /// 宴请地点
        /// </summary>
        [SugarColumn(ColumnName = "F_PLACE")]
        public string Place { get; set; }
        /// <summary>
        /// 预计费用
        /// </summary>
        [SugarColumn(ColumnName = "F_EXPECTEDCOST")]
        public decimal? ExpectedCost { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
    }
}
