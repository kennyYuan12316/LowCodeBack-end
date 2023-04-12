using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnPlaneMaterialInventory
{
    /// <summary>
    /// 库存信息数据源修改输入参数
    /// </summary>
    public class ZjnPlaneMaterialInventoryCrInput
    {
        /// <summary>
        /// 物料编码
        /// </summary>
        public string productsCode { get; set; }
        
        /// <summary>
        /// 物料名称
        /// </summary>
        public string productsName { get; set; }
        
        /// <summary>
        /// 物料数量
        /// </summary>
        public string productsQuantity { get; set; }
        
        /// <summary>
        /// 物料单位
        /// </summary>
        public string productsUnit { get; set; }
        
        /// <summary>
        /// 物料类型
        /// </summary>
        public string productsType { get; set; }
        
        /// <summary>
        /// 物料规格
        /// </summary>
        public string productsStyle { get; set; }
        
        /// <summary>
        /// 物料等级
        /// </summary>
        public string productsGrade { get; set; }
        
        /// <summary>
        /// 物料状态
        /// </summary>
        public string productsState { get; set; }
        
        /// <summary>
        /// 物料批次
        /// </summary>
        public string productsBatch { get; set; }
        
        /// <summary>
        /// 物料货位
        /// </summary>
        public string productsLocation { get; set; }
        
        /// <summary>
        /// 客户
        /// </summary>
        public string productsCustomer { get; set; }
        
        /// <summary>
        /// 供应商
        /// </summary>
        public string productsSupplier { get; set; }
        
        /// <summary>
        /// 经验类型
        /// </summary>
        public string productsCheckType { get; set; }
        
        /// <summary>
        /// 是否锁定
        /// </summary>
        public int? productsIsLock { get; set; }
        
        /// <summary>
        /// 上次盘点时间
        /// </summary>
        public DateTime? productsTakeStockTime { get; set; }
        
        /// <summary>
        /// 盘点次数
        /// </summary>
        public int? productsTakeCount { get; set; }
        
    }
}