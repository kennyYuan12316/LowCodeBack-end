using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 库存明细
    /// </summary>
    [SugarTable("zjn_base_materialInventory_detail")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnBaseMaterialInventoryDetailEntity
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
        /// 质量状态
        /// </summary>
        [SugarColumn(ColumnName = "F_Quality")]        
        public string Quality { get; set; }
        
        /// <summary>
        /// 是否冻结
        /// </summary>
        [SugarColumn(ColumnName = "F_Freeze")]        
        public string Freeze { get; set; }
        
        /// <summary>
        /// 位置
        /// </summary>
        [SugarColumn(ColumnName = "F_Location")]        
        public string Location { get; set; }
        
        /// <summary>
        /// 位置名
        /// </summary>
        [SugarColumn(ColumnName = "F_LocationName")]        
        public string LocationName { get; set; }
        
        /// <summary>
        /// 所属仓库
        /// </summary>
        [SugarColumn(ColumnName = "F_WareHouse")]        
        public string WareHouse { get; set; }
        
        /// <summary>
        /// 所属托盘
        /// </summary>
        [SugarColumn(ColumnName = "F_Tray")]        
        public string Tray { get; set; }
        
        /// <summary>
        /// 入库时间
        /// </summary>
        [SugarColumn(ColumnName = "F_EntryTime")]        
        public DateTime? EntryTime { get; set; }
        
        /// <summary>
        /// 标签条码
        /// </summary>
        [SugarColumn(ColumnName = "F_Label")]        
        public string Label { get; set; }
        
    }
}