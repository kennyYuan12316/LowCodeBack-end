using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 物料列表
    /// </summary>
    [SugarTable("zjn_base_goodsreport")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnBaseGoodsreportEntity
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
        /// 物料类型
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsType")]        
        public string ProductsType { get; set; }
        
        /// <summary>
        /// 物料单位
        /// </summary>
        [SugarColumn(ColumnName = "F_ProductsUnit")]        
        public string ProductsUnit { get; set; }
        
        /// <summary>
        /// 供应商
        /// </summary>
        [SugarColumn(ColumnName = "F_Supplier")]        
        public string Supplier { get; set; }
        
        /// <summary>
        /// 保质期
        /// </summary>
        [SugarColumn(ColumnName = "F_ExpirationDate")]        
        public string ExpirationDate { get; set; }
        
        /// <summary>
        /// 预警周期
        /// </summary>
        [SugarColumn(ColumnName = "F_WarningCycle")]        
        public string WarningCycle { get; set; }
        
    }
}