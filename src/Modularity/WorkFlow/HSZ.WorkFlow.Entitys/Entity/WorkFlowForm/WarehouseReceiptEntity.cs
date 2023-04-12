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
    /// 描 述：入库申请单
    /// </summary>
    [SugarTable("ZJN_WFORM_WAREHOUSE_RECEIPT")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class WarehouseReceiptEntity : EntityBase<string>
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
        /// 供应商名称
        /// </summary>
        [SugarColumn(ColumnName = "F_SUPPLIERNAME")]
        public string SupplierName { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        [SugarColumn(ColumnName = "F_CONTACTPHONE")]
        public string ContactPhone { get; set; }
        /// <summary>
        /// 入库类别
        /// </summary>
        [SugarColumn(ColumnName = "F_WAREHOUSCATEGORY")]
        public string WarehousCategory { get; set; }
        /// <summary>
        /// 仓库
        /// </summary>
        [SugarColumn(ColumnName = "F_WAREHOUSE")]
        public string Warehouse { get; set; }
        /// <summary>
        /// 入库人
        /// </summary>
        [SugarColumn(ColumnName = "F_WAREHOUSESPEOPLE")]
        public string WarehousesPeople { get; set; }
        /// <summary>
        /// 送货单号
        /// </summary>
        [SugarColumn(ColumnName = "F_DELIVERYNO")]
        public string DeliveryNo { get; set; }
        /// <summary>
        /// 入库单号
        /// </summary>
        [SugarColumn(ColumnName = "F_WAREHOUSENO")]
        public string WarehouseNo { get; set; }
        /// <summary>
        /// 入库日期
        /// </summary>
        [SugarColumn(ColumnName = "F_WAREHOUSDATE")]
        public DateTime? WarehousDate { get; set; }
    }
}