using System;

namespace HSZ.wms.Entitys.Dto.ZjnWmsGoodsWeight
{
    /// <summary>
    /// 物料承重配置输入参数
    /// </summary>
    public class ZjnWmsGoodsWeightListOutput
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 物料编码
        /// </summary>
        public string F_GoodsCode { get; set; }
        
        /// <summary>
        /// 最小承重
        /// </summary>
        public int? F_Min { get; set; }
        
        /// <summary>
        /// 最大承重
        /// </summary>
        public int? F_Max { get; set; }
        
        /// <summary>
        /// 物料单位
        /// </summary>
        public int? F_Unit { get; set; }
        
    }
}