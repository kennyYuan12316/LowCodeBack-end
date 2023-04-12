using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 成品出库单
    /// </summary>
    [SugarTable("zjn_report_product_out")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnReportProductOutEntity
    {
        /// <summary>
        /// F_Id
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "F_Description")]        
        public string Description { get; set; }
        
        /// <summary>
        /// 更新时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateTime")]        
        public DateTime? CreateTime { get; set; }
        
        /// <summary>
        /// 所属托盘
        /// </summary>
        [SugarColumn(ColumnName = "F_Tray")]        
        public string Tray { get; set; }
        
        /// <summary>
        /// 电芯条码
        /// </summary>
        [SugarColumn(ColumnName = "F_BatteryCode")]        
        public string BatteryCode { get; set; }
        
        /// <summary>
        /// 生产单号
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductionOrder")]        
        public string ProductionOrder { get; set; }
        
        /// <summary>
        /// 物料编码
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsCode")]        
        public string ProductsCode { get; set; }
        
        /// <summary>
        /// 物料名称
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsName")]        
        public string ProductsName { get; set; }
        
        /// <summary>
        /// 生产时间
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductionTime")]        
        public DateTime? ProductionTime { get; set; }
        
        /// <summary>
        /// 出库单
        /// </summary>
        [SugarColumn(ColumnName = "F_OutOrder")]        
        public string OutOrder { get; set; }
        
        /// <summary>
        /// 业务类型
        /// </summary>
        [SugarColumn(ColumnName = "F_BusinessType")]        
        public string BusinessType { get; set; }
        
        /// <summary>
        /// 32位批次号
        /// </summary>
        [SugarColumn(ColumnName = "F_Batch")]        
        public string Batch { get; set; }
        
        /// <summary>
        /// 出库时间
        /// </summary>
        [SugarColumn(ColumnName = "F_OutTime")]        
        public DateTime? OutTime { get; set; }
        
        /// <summary>
        /// 出库时间确认
        /// </summary>
        [SugarColumn(ColumnName = "F_OutTimeConfirm")]        
        public DateTime? OutTimeConfirm { get; set; }
        
        /// <summary>
        /// 出库站台
        /// </summary>
        [SugarColumn(ColumnName = "F_OutStation")]        
        public string OutStation { get; set; }
        
        /// <summary>
        /// 出库数量
        /// </summary>
        [SugarColumn(ColumnName = "F_OutQuantity")]        
        public int? OutQuantity { get; set; }
        
        /// <summary>
        /// 物料单位
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsUnit")]        
        public string ProductsUnit { get; set; }
        
        /// <summary>
        /// 产品标识
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductionMark")]        
        public string ProductionMark { get; set; }
        
        /// <summary>
        /// 库存标识
        /// </summary>
        [SugarColumn(ColumnName = "F_InventoryMark")]        
        public string InventoryMark { get; set; }
        
        /// <summary>
        /// 等级标识
        /// </summary>
        [SugarColumn(ColumnName = "F_ClassMark")]        
        public string ClassMark { get; set; }
        
    }
}