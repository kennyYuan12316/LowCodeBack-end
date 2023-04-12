using System;

namespace HSZ.wms.Entitys.Dto.ZjnHardList
{
    /// <summary>
    /// 设备信息输入参数
    /// </summary>
    public class ZjnHardListListOutput
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
        /// 类型
        /// </summary>
        public int? F_Type { get; set; }
        
        /// <summary>
        /// 状态
        /// </summary>
        public int? F_Status { get; set; }
        
        /// <summary>
        /// 0真实 1虚拟
        /// </summary>
        public int? F_FictitiousHard { get; set; }
        
        /// <summary>
        /// 创建者
        /// </summary>
        public string F_CreateUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreateTime { get; set; }
        
    }
}