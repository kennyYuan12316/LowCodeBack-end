using System;

namespace HSZ.wms.Entitys.Dto.ZjnWmsRegion
{
    /// <summary>
    /// 区域信息输入参数
    /// </summary>
    public class ZjnWmsRegionListOutput
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 区域编号
        /// </summary>
        public string F_RegionNo { get; set; }
        
        /// <summary>
        /// 区域名称
        /// </summary>
        public string F_RegionName { get; set; }
        
        /// <summary>
        /// 创建者
        /// </summary>
        public string F_CreateUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreateTime { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        public int? F_EnabledMark { get; set; }
        
    }
}