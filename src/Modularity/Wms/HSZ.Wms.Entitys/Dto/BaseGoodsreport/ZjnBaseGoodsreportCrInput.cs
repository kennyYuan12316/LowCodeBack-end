using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnBaseGoodsreport
{
    /// <summary>
    /// 物料列表修改输入参数
    /// </summary>
    public class ZjnBaseGoodsreportCrInput
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }
        
        /// <summary>
        /// 物料编码
        /// </summary>
        public string productsCode { get; set; }
        
        /// <summary>
        /// 物料名称
        /// </summary>
        public string productsName { get; set; }
        
        /// <summary>
        /// 物料类型
        /// </summary>
        public string productsType { get; set; }
        
        /// <summary>
        /// 物料单位
        /// </summary>
        public string productsUnit { get; set; }
        
        /// <summary>
        /// 供应商
        /// </summary>
        public string supplier { get; set; }
        
        /// <summary>
        /// 保质期
        /// </summary>
        public string expirationDate { get; set; }
        
        /// <summary>
        /// 预警周期
        /// </summary>
        public string warningCycle { get; set; }
        
    }
}