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
    /// 描 述：合同开票流程
    /// </summary>
    [SugarTable("ZJN_WFORM_CONTRACT_BILLING")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ConBillingEntity : EntityBase<string>
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
        /// 开票人
        /// </summary>
        [SugarColumn(ColumnName = "F_DRAWER")]
        public string Drawer { get; set; }
        /// <summary>
        /// 开票日期
        /// </summary>
        [SugarColumn(ColumnName = "F_BILLDATE")]
        public DateTime? BillDate { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        [SugarColumn(ColumnName = "F_COMPANYNAME")]
        public string CompanyName { get; set; }
        /// <summary>
        /// 关联名称
        /// </summary>
        [SugarColumn(ColumnName = "F_CONNAME")]
        public string ConName { get; set; }
        /// <summary>
        /// 开户银行
        /// </summary>
        [SugarColumn(ColumnName = "F_BANK")]
        public string Bank { get; set; }
        /// <summary>
        /// 开户账号
        /// </summary>
        [SugarColumn(ColumnName = "F_AMOUNT")]
        public string Amount { get; set; }
        /// <summary>
        /// 开票金额
        /// </summary>
        [SugarColumn(ColumnName = "F_BILLAMOUNT")]
        public decimal? BillAmount { get; set; }
        /// <summary>
        /// 税号
        /// </summary>
        [SugarColumn(ColumnName = "F_TAXID")]
        public string TaxId { get; set; }
        /// <summary>
        /// 发票号
        /// </summary>
        [SugarColumn(ColumnName = "F_INVOICEID")]
        public string InvoiceId { get; set; }
        /// <summary>
        /// 发票地址
        /// </summary>
        [SugarColumn(ColumnName = "F_INVOADDRESS")]
        public string InvoAddress { get; set; }
        /// <summary>
        /// 付款金额
        /// </summary>
        [SugarColumn(ColumnName = "F_PAYAMOUNT")]
        public decimal? PayAmount { get; set; }
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
