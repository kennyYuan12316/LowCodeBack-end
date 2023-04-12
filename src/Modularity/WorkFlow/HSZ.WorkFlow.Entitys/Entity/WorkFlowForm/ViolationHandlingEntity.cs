using HSZ.Common.Const;
using HSZ.Common.Entity;
using SqlSugar;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSZ.WorkFlow.Entitys
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：违章处理申请表
    /// </summary>
    [SugarTable("ZJN_WFORM_VIOLATION_HANDLING")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ViolationHandlingEntity : EntityBase<string>
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
        /// 车牌号
        /// </summary>
        [SugarColumn(ColumnName = "F_PLATENUM")]
        public string PlateNum { get; set; }
        /// <summary>
        /// 驾驶人
        /// </summary>
        [SugarColumn(ColumnName = "F_DRIVER")]
        public string Driver { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        [SugarColumn(ColumnName = "F_LEADINGOFFICIAL")]
        public string LeadingOfficial { get; set; }
        /// <summary>
        /// 违章日期
        /// </summary>
        [SugarColumn(ColumnName = "F_PECCANCYDATE")]
        public DateTime? PeccancyDate { get; set; }
        /// <summary>
        /// 通知日期
        /// </summary>
        [SugarColumn(ColumnName = "F_NOTICEDATE")]
        public DateTime? NoticeDate { get; set; }
        /// <summary>
        /// 限处理日期
        /// </summary>
        [SugarColumn(ColumnName = "F_LIMITDATE")]
        public DateTime? LimitDate { get; set; }
        /// <summary>
        /// 违章地点
        /// </summary>
        [SugarColumn(ColumnName = "F_VIOLATIONSITE")]
        public string ViolationSite { get; set; }
        /// <summary>
        /// 违章行为
        /// </summary>
        [SugarColumn(ColumnName = "F_VIOLATIONBEHAVIOR")]
        public string ViolationBehavior { get; set; }
        /// <summary>
        /// 违章扣分
        /// </summary>
        [SugarColumn(ColumnName = "F_DEDUCTION")]
        public string Deduction { get; set; }
        /// <summary>
        /// 违章罚款
        /// </summary>
        [SugarColumn(ColumnName = "F_AMOUNTMONEY")]
        public decimal? AmountMoney { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
    }
}
