using System;

namespace HSZ.wms.Entitys.Dto.ZjnBaseAisle
{
    /// <summary>
    /// 巷道信息输入参数
    /// </summary>
    public class ZjnBaseAisleListOutput
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 巷道编号
        /// </summary>
        public string F_AisleNo { get; set; }
        
        /// <summary>
        /// 巷道名称
        /// </summary>
        public string F_AisleName { get; set; }
        
        /// <summary>
        /// 区域编号
        /// </summary>
        public string F_RegionNo { get; set; }
        
        /// <summary>
        /// 仓库编号
        /// </summary>
        public string F_WarehouseNo { get; set; }
        
        /// <summary>
        /// 堆垛机编号
        /// </summary>
        public string F_StackerNo { get; set; }
        
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