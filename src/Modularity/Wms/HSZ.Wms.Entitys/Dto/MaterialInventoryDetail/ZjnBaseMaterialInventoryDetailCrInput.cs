using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnBaseMaterialInventoryDetail
{
    /// <summary>
    /// 库存明细修改输入参数
    /// </summary>
    public class ZjnBaseMaterialInventoryDetailCrInput
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
        /// 物料单位
        /// </summary>
        public string productsUnit { get; set; }
        
        /// <summary>
        /// 32位批次号
        /// </summary>
        public string batch { get; set; }
        
        /// <summary>
        /// 质量状态
        /// </summary>
        public string quality { get; set; }
        
        /// <summary>
        /// 是否冻结
        /// </summary>
        public string freeze { get; set; }
        
        /// <summary>
        /// 位置
        /// </summary>
        public string location { get; set; }
        
        /// <summary>
        /// 位置名
        /// </summary>
        public string locationName { get; set; }
        
        /// <summary>
        /// 所属仓库
        /// </summary>
        public string wareHouse { get; set; }
        
        /// <summary>
        /// 所属托盘
        /// </summary>
        public string tray { get; set; }
        
        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime? entryTime { get; set; }
        
        /// <summary>
        /// 标签条码
        /// </summary>
        public string label { get; set; }
        
    }
}