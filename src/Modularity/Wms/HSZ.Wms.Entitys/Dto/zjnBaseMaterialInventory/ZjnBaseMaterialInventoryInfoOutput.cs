using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnBaseMaterialInventory
{
    /// <summary>
    /// 立库库存信息输出参数
    /// </summary>
    public class ZjnBaseMaterialInventoryInfoOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 创建用户
        /// </summary>
        public string createUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createTime { get; set; }
        
        /// <summary>
        /// 修改用户
        /// </summary>
        public string lastModifyUserId { get; set; }
        
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? lastModifyTime { get; set; }
        
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
        public decimal productsQuantity { get; set; }
        
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
        
        /// <summary>
        /// 扩展字段
        /// </summary>
        public string expand { get; set; }
        
    }
}