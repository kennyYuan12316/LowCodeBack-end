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
    /// 描 述：收入确认分析表
    /// </summary>
    [SugarTable("ZJN_WFORM_INCOMERE_COGNITION")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class IncomeRecognitionEntity : EntityBase<string>
    {
        /// <summary>
        /// 流程主题
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
        /// 结算月份
        /// </summary>
        [SugarColumn(ColumnName = "F_SETTLEMENTMONTH")]
        public string SettlementMonth { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        [SugarColumn(ColumnName = "F_CUSTOMERNAME")]
        public string CustomerName { get; set; }
        /// <summary>
        /// 合同编码
        /// </summary>
        [SugarColumn(ColumnName = "F_CONTRACTNUM")]
        public string ContractNum { get; set; }
        /// <summary>
        /// 合同金额
        /// </summary>
        [SugarColumn(ColumnName = "F_TOTALAMOUNT")]
        public decimal? TotalAmount { get; set; }
        /// <summary>
        /// 到款银行
        /// </summary>
        [SugarColumn(ColumnName = "F_MONEYBANK")]
        public string MoneyBank { get; set; }
        /// <summary>
        /// 到款金额
        /// </summary>
        [SugarColumn(ColumnName = "F_ACTUALAMOUNT")]
        public decimal? ActualAmount { get; set; }
        /// <summary>
        /// 到款日期
        /// </summary>
        [SugarColumn(ColumnName = "F_PAYMENTDATE")]
        public DateTime? PaymentDate { get; set; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        [SugarColumn(ColumnName = "F_CONTACTNAME")]
        public string ContactName { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        [SugarColumn(ColumnName = "F_CONTACPHONE")]
        public string ContacPhone { get; set; }
        /// <summary>
        /// 联系QQ
        /// </summary>
        [SugarColumn(ColumnName = "F_CONTACTQQ")]
        public string ContactQQ { get; set; }
        /// <summary>
        /// 未付金额
        /// </summary>
        [SugarColumn(ColumnName = "F_UNPAIDAMOUNT")]
        public decimal? UnpaidAmount { get; set; }
        /// <summary>
        /// 已付金额
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
