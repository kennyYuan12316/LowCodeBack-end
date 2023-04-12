using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsGoodsWeight
{
    /// <summary>
    /// 物料承重配置修改输入参数
    /// </summary>
    public class ZjnWmsGoodsWeightCrInput
    {
        /// <summary>
        /// 物料编码
        /// </summary>
        public string goodsCode { get; set; }
        
        /// <summary>
        /// 最小承重
        /// </summary>
        public int? min { get; set; }
        
        /// <summary>
        /// 最大承重
        /// </summary>
        public int? max { get; set; }
        
        /// <summary>
        /// 物料单位
        /// </summary>
        public string unit { get; set; }
        
    }
}