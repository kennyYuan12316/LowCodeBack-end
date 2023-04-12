using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 变更列表
    /// </summary>
    [SugarTable("zjn_base_changeorder")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnBaseChangeorderEntity
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
        /// 变更单
        /// </summary>
        [SugarColumn(ColumnName = "F_ChangeOrder")]        
        public string ChangeOrder { get; set; }
        
        /// <summary>
        /// 更新时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateTime")]        
        public DateTime? CreateTime { get; set; }
        
        /// <summary>
        /// 创建用户
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateUser")]        
        public string CreateUser { get; set; }
        
        /// <summary>
        /// 业务类型
        /// </summary>
        [SugarColumn(ColumnName = "F_BusinessType")]        
        public string BusinessType { get; set; }
        
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
        /// 物料编码转移前
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsCodeAgo")]        
        public string ProductsCodeAgo { get; set; }
        
        /// <summary>
        /// 物料名称转移前
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsNameAgo")]        
        public string ProductsNameAgo { get; set; }
        
        /// <summary>
        /// 批次号转移前
        /// </summary>
        [SugarColumn(ColumnName = "F_BatchAgo")]        
        public string BatchAgo { get; set; }
        
        /// <summary>
        /// 库存状态转移前
        /// </summary>
        [SugarColumn(ColumnName = "F_InventoryStatusAgo")]        
        public string InventoryStatusAgo { get; set; }
        
        /// <summary>
        /// 物料编码转移后
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsCodeAfter")]        
        public string ProductsCodeAfter { get; set; }
        
        /// <summary>
        /// 物料名称转移后
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsNameAfter")]        
        public string ProductsNameAfter { get; set; }
        
        /// <summary>
        /// 批次号转移后
        /// </summary>
        [SugarColumn(ColumnName = "F_BatchAfter")]        
        public string BatchAfter { get; set; }
        
        /// <summary>
        /// 库存状态转移后
        /// </summary>
        [SugarColumn(ColumnName = "F_InventoryStatusAfter")]        
        public string InventoryStatusAfter { get; set; }
        
    }
}