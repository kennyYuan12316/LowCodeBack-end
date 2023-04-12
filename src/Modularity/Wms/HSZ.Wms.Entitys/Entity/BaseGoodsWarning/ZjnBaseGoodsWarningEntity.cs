using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 物料库存预警
    /// </summary>
    [SugarTable("zjn_base_goods_warning")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnBaseGoodsWarningEntity
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
        /// 物料数量
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsQuantity")]        
        public int? ProductsQuantity { get; set; }
        
        /// <summary>
        /// 物料单位
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsUnit")]        
        public string ProductsUnit { get; set; }
        
        /// <summary>
        /// 32位批次号
        /// </summary>
        [SugarColumn(ColumnName = "F_Batch")]        
        public string Batch { get; set; }
        
        /// <summary>
        /// 入库时间
        /// </summary>
        [SugarColumn(ColumnName = "F_EntryTime")]        
        public DateTime? EntryTime { get; set; }
        
        /// <summary>
        /// 物料类型
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsType")]        
        public string ProductsType { get; set; }
        
        /// <summary>
        /// 供应商
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsSupplier")]        
        public string ProductsSupplier { get; set; }
        
        /// <summary>
        /// 库存上限
        /// </summary>
        [SugarColumn(ColumnName = "F_InventoryMax")]        
        public int? InventoryMax { get; set; }
        
        /// <summary>
        /// 库存下限
        /// </summary>
        [SugarColumn(ColumnName = "F_InventoryMin")]        
        public int? InventoryMin { get; set; }
        
        /// <summary>
        /// 预警原因
        /// </summary>
        [SugarColumn(ColumnName = "F_WarningResult")]        
        public string WarningResult { get; set; }
        
        /// <summary>
        /// 保质期
        /// </summary>
        [SugarColumn(ColumnName = "F_ExpirationDate")]        
        public string ExpirationDate { get; set; }
        
        /// <summary>
        /// 生产时间
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductionTime")]        
        public DateTime? ProductionTime { get; set; }
        
        /// <summary>
        /// 失效时间
        /// </summary>
        [SugarColumn(ColumnName = "F_FailureTime")]        
        public DateTime? FailureTime { get; set; }
        
        /// <summary>
        /// 预警周期
        /// </summary>
        [SugarColumn(ColumnName = "F_WarningCycle")]        
        public string WarningCycle { get; set; }
        
        /// <summary>
        /// 保质期预警
        /// </summary>
        [SugarColumn(ColumnName = "F_WarningTime")]        
        public DateTime? WarningTime { get; set; }
        
    }
}