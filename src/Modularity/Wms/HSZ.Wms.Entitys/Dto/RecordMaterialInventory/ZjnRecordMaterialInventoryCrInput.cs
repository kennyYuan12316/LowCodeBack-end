using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnRecordMaterialInventory
{
    /// <summary>
    /// 库存流水修改输入参数
    /// </summary>
    public class ZjnRecordMaterialInventoryCrInput
    {
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
        /// 32位批次号
        /// </summary>
        public string batch { get; set; }
        
        /// <summary>
        /// 质量状态
        /// </summary>
        public string quality { get; set; }
        
        /// <summary>
        /// 位置
        /// </summary>
        public string location { get; set; }
        
        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime? entryTime { get; set; }
        
        /// <summary>
        /// 订单
        /// </summary>
        public string order { get; set; }
        
        /// <summary>
        /// 业务类型
        /// </summary>
        public string businessType { get; set; }
        
        /// <summary>
        /// 入库单
        /// </summary>
        public string entryOrder { get; set; }
        
        /// <summary>
        /// 出库单
        /// </summary>
        public string outOrder { get; set; }
        
        /// <summary>
        /// 操作
        /// </summary>
        public string operation { get; set; }
        
    }
}