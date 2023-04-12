using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 库存信息数据源
    /// </summary>
    [SugarTable("zjn_wms_materialInventory")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnPlaneMaterialInventoryEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 创建用户
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateUser")]        
        public string CreateUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateTime")]        
        public DateTime? CreateTime { get; set; }
        
        /// <summary>
        /// 修改用户
        /// </summary>
        [SugarColumn(ColumnName = "F_LastModifyUserId")]        
        public string LastModifyUserId { get; set; }
        
        /// <summary>
        /// 修改时间
        /// </summary>
        [SugarColumn(ColumnName = "F_LastModifyTime")]        
        public DateTime? LastModifyTime { get; set; }
        
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
        public decimal? ProductsQuantity { get; set; }
        
        /// <summary>
        /// 物料单位
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsUnit")]        
        public string ProductsUnit { get; set; }
        
        /// <summary>
        /// 物料类型
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsType")]        
        public string ProductsType { get; set; }
        
        /// <summary>
        /// 物料规格
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsStyle")]        
        public string ProductsStyle { get; set; }
        
        /// <summary>
        /// 物料等级
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsGrade")]        
        public string ProductsGrade { get; set; }
        
        /// <summary>
        /// 物料状态
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsState")]        
        public string ProductsState { get; set; }
        
        /// <summary>
        /// 物料批次
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsBatch")]        
        public string ProductsBatch { get; set; }
        
        /// <summary>
        /// 物料货位
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsLocation")]        
        public string ProductsLocation { get; set; }
        
        /// <summary>
        /// 客户
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsCustomer")]        
        public string ProductsCustomer { get; set; }
        
        /// <summary>
        /// 供应商
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsSupplier")]        
        public string ProductsSupplier { get; set; }
        
        /// <summary>
        /// 经验类型
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsCheckType")]        
        public string ProductsCheckType { get; set; }
        
        /// <summary>
        /// 是否锁定
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsIsLock")]        
        public int? ProductsIsLock { get; set; }
        
        /// <summary>
        /// 上次盘点时间
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsTakeStockTime")]        
        public DateTime? ProductsTakeStockTime { get; set; }
        
        /// <summary>
        /// 盘点次数
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsTakeCount")]        
        public int? ProductsTakeCount { get; set; }


        /// <summary>
        /// 锁定原因
        /// </summary>
        [SugarColumn(ColumnName = "F_LockReason")]
        public string LockReason { get; set; }

        /// <summary>
        /// 检验状态
        /// </summary>
        [SugarColumn(ColumnName = "F_InspectionStatus")]
        public string InspectionStatus { get; set; }

        /// <summary>
        /// 生产日期
        /// </summary>
        [SugarColumn(ColumnName = "F_ExpiryDate")]
        public DateTime? ExpiryDate { get; set; }
    
        /// <summary>
        /// 备用字段
        /// </summary>
        [SugarColumn(ColumnName = "case1")]        
        public string Case1 { get; set; }
        
        /// <summary>
        /// 备用字段
        /// </summary>
        [SugarColumn(ColumnName = "case2")]        
        public string Case2 { get; set; }
        
        /// <summary>
        /// 备用字段
        /// </summary>
        [SugarColumn(ColumnName = "case3")]        
        public string Case3 { get; set; }
        
        /// <summary>
        /// 备用字段
        /// </summary>
        [SugarColumn(ColumnName = "case4")]        
        public string Case4 { get; set; }
        
        /// <summary>
        /// 备用字段
        /// </summary>
        [SugarColumn(ColumnName = "case5")]        
        public string Case5 { get; set; }
        

        
    }
}