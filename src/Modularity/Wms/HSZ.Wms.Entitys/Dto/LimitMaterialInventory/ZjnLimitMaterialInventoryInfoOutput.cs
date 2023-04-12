using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnLimitMaterialInventory
{
    /// <summary>
    /// 物料库存上下限输出参数
    /// </summary>
    public class ZjnLimitMaterialInventoryInfoOutput
    {
        /// <summary>
        /// F_Id
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }
        
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? createTime { get; set; }
        
        /// <summary>
        /// 所属仓库编码
        /// </summary>
        public string wareHouseCode { get; set; }
        
        /// <summary>
        /// 所属仓库
        /// </summary>
        public string wareHouse { get; set; }
        
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
        public int? productsQuantity { get; set; }
        
        /// <summary>
        /// 物料单位
        /// </summary>
        public string productsUnit { get; set; }
        
        /// <summary>
        /// 库存上限
        /// </summary>
        public int? inventoryMax { get; set; }
        
        /// <summary>
        /// 库存下限
        /// </summary>
        public int? inventoryMin { get; set; }
        
    }
}