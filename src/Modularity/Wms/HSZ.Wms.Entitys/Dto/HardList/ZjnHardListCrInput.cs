using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnHardList
{
    /// <summary>
    /// 设备信息修改输入参数
    /// </summary>
    public class ZjnHardListCrInput
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string hardNo { get; set; }
        
        /// <summary>
        /// 设备名称
        /// </summary>
        public string hardName { get; set; }
        
        /// <summary>
        /// 类型
        /// </summary>
        public string type { get; set; }
        
        /// <summary>
        /// 状态
        /// </summary>
        public int? status { get; set; }
        
        /// <summary>
        /// 0真实 1虚拟
        /// </summary>
        public string fictitiousHard { get; set; }
        
        /// <summary>
        /// 创建者
        /// </summary>
        public string createUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createTime { get; set; }
        
    }
}