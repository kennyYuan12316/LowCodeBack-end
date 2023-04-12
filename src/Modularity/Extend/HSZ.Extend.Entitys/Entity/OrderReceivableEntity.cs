using HSZ.Common.Const;
using HSZ.Common.Entity;
using SqlSugar;
using System;

namespace HSZ.Extend.Entitys
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：订单收款
    /// </summary>
    [SugarTable("ZJN_EXT_ORDER_RECEIVABLE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class OrderReceivableEntity : EntityBase<string>
    {
        /// <summary>
        /// 订单主键
        /// </summary>
        [SugarColumn(ColumnName = "F_ORDERID")]
        public string OrderId { get; set; }
        /// <summary>
        /// 收款摘要
        /// </summary>
        [SugarColumn(ColumnName = "F_ABSTRACT")]
        public string Abstract { get; set; }
        /// <summary>
        /// 收款日期
        /// </summary>
        [SugarColumn(ColumnName = "F_RECEIVABLEDATE")]
        public DateTime? ReceivableDate { get; set; }
        /// <summary>
        /// 收款比率
        /// </summary>
        [SugarColumn(ColumnName = "F_RECEIVABLERATE")]
        public decimal? ReceivableRate { get; set; }
        /// <summary>
        /// 收款金额
        /// </summary>
        [SugarColumn(ColumnName = "F_RECEIVABLEMONEY")]
        public decimal? ReceivableMoney { get; set; }
        /// <summary>
        /// 收款方式
        /// </summary>
        [SugarColumn(ColumnName = "F_RECEIVABLEMODE")]
        public string ReceivableMode { get; set; }
        /// <summary>
        /// 收款状态
        /// </summary>
        [SugarColumn(ColumnName = "F_RECEIVABLESTATE")]
        public int? ReceivableState { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        [SugarColumn(ColumnName = "F_SORTCODE")]
        public long? SortCode { get; set; }
    }
}