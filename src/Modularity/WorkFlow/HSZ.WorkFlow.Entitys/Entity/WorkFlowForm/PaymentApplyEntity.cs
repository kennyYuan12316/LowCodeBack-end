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
    /// 描 述：付款申请单
    /// </summary>
    [SugarTable("ZJN_WFORM_PAYMENT_APPLY")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class PaymentApplyEntity : EntityBase<string>
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
        /// 申请部门
        /// </summary>
        [SugarColumn(ColumnName = "F_DEPARTMENTAL")]
        public string Departmental { get; set; }
        /// <summary>
        /// 申请日期
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYDATE")]
        public DateTime? ApplyDate { get; set; }
        /// <summary>
        /// 用途名称
        /// </summary>
        [SugarColumn(ColumnName = "F_PURPOSENAME")]
        public string PurposeName { get; set; }
        /// <summary>
        /// 项目类别
        /// </summary>
        [SugarColumn(ColumnName = "F_PROJECTCATEGORY")]
        public string ProjectCategory { get; set; }
        /// <summary>
        /// 项目负责人
        /// </summary>
        [SugarColumn(ColumnName = "F_PROJECTLEADER")]
        public string ProjectLeader { get; set; }
        /// <summary>
        /// 开户银行
        /// </summary>
        [SugarColumn(ColumnName = "F_OPENINGBANK")]
        public string OpeningBank { get; set; }
        /// <summary>
        /// 收款账号
        /// </summary>
        [SugarColumn(ColumnName = "F_BENEFICIARYACCOUNT")]
        public string BeneficiaryAccount { get; set; }
        /// <summary>
        /// 联系方式
        /// </summary>
        [SugarColumn(ColumnName = "F_RECEIVABLECONTACT")]
        public string ReceivableContact { get; set; }
        /// <summary>
        /// 付款单位
        /// </summary>
        [SugarColumn(ColumnName = "F_PAYMENTUNIT")]
        public string PaymentUnit { get; set; }
        /// <summary>
        /// 申请金额
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYAMOUNT")]
        public decimal? ApplyAmount { get; set; }
        /// <summary>
        /// 结算方式
        /// </summary>
        [SugarColumn(ColumnName = "F_SETTLEMENTMETHOD")]
        public string SettlementMethod { get; set; }
        /// <summary>
        /// 付款类型
        /// </summary>
        [SugarColumn(ColumnName = "F_PAYMENTTYPE")]
        public string PaymentType { get; set; }
        /// <summary>
        /// 付款金额
        /// </summary>
        [SugarColumn(ColumnName = "F_AMOUNTPAID")]
        public decimal? AmountPaid { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
    }
}
