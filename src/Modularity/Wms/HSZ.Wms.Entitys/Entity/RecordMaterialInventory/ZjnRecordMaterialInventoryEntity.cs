using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 库存流水
    /// </summary>
    [SugarTable("zjn_record_materialInventory")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnRecordMaterialInventoryEntity
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
        /// 位置
        /// </summary>
        [SugarColumn(ColumnName = "F_Location")]        
        public string Location { get; set; }
        
        /// <summary>
        /// 入库时间
        /// </summary>
        [SugarColumn(ColumnName = "F_EntryTime")]        
        public DateTime? EntryTime { get; set; }
        
        /// <summary>
        /// 订单
        /// </summary>
        [SugarColumn(ColumnName = "F_Order")]        
        public string Order { get; set; }
        
        /// <summary>
        /// 业务类型
        /// </summary>
        [SugarColumn(ColumnName = "F_BusinessType")]        
        public string BusinessType { get; set; }
        
        /// <summary>
        /// 入库单
        /// </summary>
        [SugarColumn(ColumnName = "F_EntryOrder")]        
        public string EntryOrder { get; set; }
        
        /// <summary>
        /// 出库单
        /// </summary>
        [SugarColumn(ColumnName = "F_OutOrder")]        
        public string OutOrder { get; set; }
        
        /// <summary>
        /// 操作
        /// </summary>
        [SugarColumn(ColumnName = "F_Operation")]        
        public string Operation { get; set; }
        
    }
}