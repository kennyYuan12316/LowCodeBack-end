using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnBaseGoodsWarning
{
    /// <summary>
    /// 物料库存预警输出参数
    /// </summary>
    public class ZjnBaseGoodsWarningInfoOutput
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
        /// 32位批次号
        /// </summary>
        public string batch { get; set; }
        
        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime? entryTime { get; set; }
        
        /// <summary>
        /// 物料类型
        /// </summary>
        public string productsType { get; set; }
        
        /// <summary>
        /// 供应商
        /// </summary>
        public string productsSupplier { get; set; }
        
        /// <summary>
        /// 库存上限
        /// </summary>
        public int? inventoryMax { get; set; }
        
        /// <summary>
        /// 库存下限
        /// </summary>
        public int? inventoryMin { get; set; }
        
        /// <summary>
        /// 预警原因
        /// </summary>
        public string warningResult { get; set; }
        
        /// <summary>
        /// 保质期
        /// </summary>
        public string expirationDate { get; set; }
        
        /// <summary>
        /// 生产时间
        /// </summary>
        public DateTime? productionTime { get; set; }
        
        /// <summary>
        /// 失效时间
        /// </summary>
        public DateTime? failureTime { get; set; }
        
        /// <summary>
        /// 预警周期
        /// </summary>
        public string warningCycle { get; set; }
        
        /// <summary>
        /// 保质期预警
        /// </summary>
        public DateTime? warningTime { get; set; }
        
    }
}