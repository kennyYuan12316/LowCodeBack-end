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
    /// 描 述：采购原材料
    /// </summary>
    [SugarTable("ZJN_WFORM_PROCUREMENT_MATERIAL")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ProcurementMaterialEntity : EntityBase<string>
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
        /// 采购单位
        /// </summary>
        [SugarColumn(ColumnName = "F_PURCHASEUNIT")]
        public string PurchaseUnit { get; set; }
        /// <summary>
        /// 送货方式
        /// </summary>
        [SugarColumn(ColumnName = "F_DELIVERYMODE")]
        public string DeliveryMode { get; set; }
        /// <summary>
        /// 送货地址
        /// </summary>
        [SugarColumn(ColumnName = "F_DELIVERYADDRESS")]
        public string DeliveryAddress { get; set; }
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
        /// 相关附件
        /// </summary>
        [SugarColumn(ColumnName = "F_FILEJSON")]
        public string FileJson { get; set; }
        /// <summary>
        /// 用途原因
        /// </summary>
        [SugarColumn(ColumnName = "F_REASON")]
        public string Reason { get; set; }
    }
}