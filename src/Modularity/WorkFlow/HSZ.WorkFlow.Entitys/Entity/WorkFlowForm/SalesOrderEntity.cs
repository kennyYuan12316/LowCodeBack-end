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
    /// 描 述：销售订单
    /// </summary>
    [SugarTable("ZJN_WFORM_SALE_ORDER")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class SalesOrderEntity : EntityBase<string>
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
        /// 流程等级
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWURGENT")]
        public int? FlowUrgent { get; set; }
        /// <summary>
        /// 流程单据
        /// </summary>
        [SugarColumn(ColumnName = "F_BILLNO")]
        public string BillNo { get; set; }
        /// <summary>
        /// 业务人员
        /// </summary>
        [SugarColumn(ColumnName = "F_SALESMAN")]
        public string Salesman { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        [SugarColumn(ColumnName = "F_CUSTOMERNAME")]
        public string CustomerName { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        [SugarColumn(ColumnName = "F_CONTACTS")]
        public string Contacts { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        [SugarColumn(ColumnName = "F_CONTACTPHONE")]
        public string ContactPhone { get; set; }
        /// <summary>
        /// 客户地址
        /// </summary>
        [SugarColumn(ColumnName = "F_CUSTOMERADDRES")]
        public string CustomerAddres { get; set; }
        /// <summary>
        /// 发票编码
        /// </summary>
        [SugarColumn(ColumnName = "F_TICKETNUM")]
        public string TicketNum { get; set; }
        /// <summary>
        /// 开票日期
        /// </summary>
        [SugarColumn(ColumnName = "F_TICKETDATE")]
        public DateTime? TicketDate { get; set; }
        /// <summary>
        /// 发票类型
        /// </summary>
        [SugarColumn(ColumnName = "F_INVOICETYPE")]
        public string InvoiceType { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        [SugarColumn(ColumnName = "F_PAYMENTMETHOD")]
        public string PaymentMethod { get; set; }
        /// <summary>
        /// 付款金额
        /// </summary>
        [SugarColumn(ColumnName = "F_PAYMENTMONEY")]
        public decimal? PaymentMoney { get; set; }
        /// <summary>
        /// 销售日期
        /// </summary>
        [SugarColumn(ColumnName = "F_SALESDATE")]
        public DateTime? SalesDate { get; set; }
        /// <summary>
        /// 相关附件
        /// </summary>
        [SugarColumn(ColumnName = "F_FILEJSON")]
        public string FileJson { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
    }
}