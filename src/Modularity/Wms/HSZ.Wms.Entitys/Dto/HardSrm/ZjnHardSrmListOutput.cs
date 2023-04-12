using System;

namespace HSZ.wms.Entitys.Dto.ZjnHardSrm
{
    /// <summary>
    /// 堆垛机信息输入参数
    /// </summary>
    public class ZjnHardSrmListOutput
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 设备编号
        /// </summary>
        public string F_HardNo { get; set; }
        
        /// <summary>
        /// 设备名称
        /// </summary>
        public string F_HardName { get; set; }
        
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