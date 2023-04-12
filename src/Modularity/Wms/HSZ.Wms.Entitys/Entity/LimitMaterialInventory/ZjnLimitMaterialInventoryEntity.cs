using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 物料库存上下限
    /// </summary>
    [SugarTable("zjn_limit_materialInventory")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnLimitMaterialInventoryEntity
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
        /// 所属仓库编码
        /// </summary>
        [SugarColumn(ColumnName = "F_WareHouseCode")]        
        public string WareHouseCode { get; set; }
        
        /// <summary>
        /// 所属仓库
        /// </summary>
        [SugarColumn(ColumnName = "F_WareHouse")]        
        public string WareHouse { get; set; }
        
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
        /// 库存上限
        /// </summary>
        [SugarColumn(ColumnName = "F_InventoryMax")]        
        public int? InventoryMax { get; set; }
        
        /// <summary>
        /// 库存下限
        /// </summary>
        [SugarColumn(ColumnName = "F_InventoryMin")]        
        public int? InventoryMin { get; set; }
        
    }
}